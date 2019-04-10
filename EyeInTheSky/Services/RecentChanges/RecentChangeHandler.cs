namespace EyeInTheSky.Services.RecentChanges
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;

    public class RecentChangeHandler : IRecentChangeHandler
    {
        private readonly IAppConfiguration appConfig;
        private readonly ILogger logger;
        private readonly IChannelConfiguration channelConfig;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IIrcClient freenodeClient;
        private readonly IEmailHelper emailHelper;
        private readonly INotificationTemplates templates;

        public RecentChangeHandler(
            IAppConfiguration appConfig,
            ILogger logger,
            IChannelConfiguration channelConfig,
            IBotUserConfiguration botUserConfiguration,
            IIrcClient freenodeClient,
            IEmailHelper emailHelper,
            INotificationTemplates templates)
        {
            this.appConfig = appConfig;
            this.logger = logger;
            this.channelConfig = channelConfig;
            this.botUserConfiguration = botUserConfiguration;
            this.freenodeClient = freenodeClient;
            this.emailHelper = emailHelper;
            this.templates = templates;

            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Warn("Not sending email; email configuration is disabled");
            }
        }

        public void HandleRcEvent(MessageReceivedEventArgs e, IRecentChange rc)
        {
            var stalks = this.channelConfig.MatchStalks(rc, e.Target).ToList();

            if (stalks.Count == 0)
            {
                return;
            }

            this.logger.InfoFormat(
                "Seen stalked change for stalks: {0}",
                string.Join(" ", stalks.Select(x => x.Identifier)));

            // Touch expiry date (*before* email is sent)
            foreach (var stalk in stalks)
            {
                stalk.TriggerDynamicExpiry();
            }

            // send notifications
            this.SendToIrc(stalks, rc);
            this.SendEmail(stalks, rc);

            // touch update/count
            foreach (var stalk in stalks)
            {
                stalk.LastTriggerTime = DateTime.UtcNow;
                stalk.TriggerCount++;
            }

            this.channelConfig.Save();
        }

        private void SendEmail(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Debug("Not sending email; email configuration is disabled");
                return;
            }

            var stalkSplit = this.botUserConfiguration.Items.ToDictionary(x => x, y => new HashSet<IStalk>());

            foreach (var stalk in stalks)
            {
                // channel subscribers
                foreach (var channelUser in this.channelConfig[stalk.Channel].Users.Where(x => x.Subscribed))
                {
                    var botUser = this.botUserConfiguration[channelUser.Mask.ToString()];
                    stalkSplit[botUser].Add(stalk);
                }

                // stalk subscribers
                foreach (var stalkUser in stalk.Subscribers)
                {
                    var botUser = this.botUserConfiguration[stalkUser.Mask.ToString()];

                    if (stalkUser.Subscribed)
                    {
                        stalkSplit[botUser].Add(stalk);
                    }
                    else
                    {
                        // subscription exclusion for channel users
                        stalkSplit[botUser].Remove(stalk);
                    }
                }
            }

            foreach (var kvp in stalkSplit)
            {
                var stalkList = kvp.Value;
                var botUser = kvp.Key;

                if (!stalkList.Any())
                {
                    continue;
                }

                if (!botUser.EmailAddressConfirmed)
                {
                    continue;
                }

                this.SendIndividualEmail(stalkList.ToList(), rc, botUser);
            }
        }

        private void SendIndividualEmail(IList<IStalk> stalks, IRecentChange rc, IBotUser botUser)
        {
            if (!botUser.EmailAddressConfirmed)
            {
                return;
            }

            var stalkList = string.Join(", ", stalks.Select(x => x.Identifier));

            try
            {

                var extraHeaders = new Dictionary<string, string>
                {
                    {"StalkList", stalkList}
                };

                this.emailHelper.SendEmail(
                    this.FormatMessageForEmail(stalks, rc, botUser),
                    string.Format(this.templates.EmailRcSubject, stalkList, rc.Page),
                    null,
                    botUser,
                    extraHeaders);
            }
            catch (Exception ex)
            {
                this.logger.ErrorFormat(ex, "Failed to send notification email for RC {0}", rc);
            }
        }

        private void SendToIrc(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            var splitStalks = new Dictionary<string, List<IStalk>>();

            foreach (var stalk in stalks)
            {
                if (!splitStalks.ContainsKey(stalk.Channel))
                {
                    splitStalks.Add(stalk.Channel, new List<IStalk>());
                }

                splitStalks[stalk.Channel].Add(stalk);
            }

            try
            {
                foreach (var stalkList in splitStalks)
                {
                    this.freenodeClient.SendMessage(stalkList.Key, this.FormatMessageForIrc(stalkList.Value, rc));
                }
            }
            catch (Exception ex)
            {
                this.logger.ErrorFormat(ex, "Failed to send notification IRC message for RC {0}", rc);
            }
        }

        public string FormatMessageForIrc(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            var stalkTags = new StringBuilder();
            bool first = true;
            foreach (var s in stalks)
            {
                if (!first)
                {
                    stalkTags.Append(this.templates.IrcStalkTagSeparator);
                }

                first = false;

                stalkTags.Append(s.Identifier);
            }

            var sizeDiff = "N/A";
            if (rc.SizeDiff.HasValue)
            {
                sizeDiff = (rc.SizeDiff.Value > 0 ? "+" : string.Empty) +
                           rc.SizeDiff.Value.ToString(CultureInfo.InvariantCulture);
            }

            return string.Format(
                this.templates.IrcAlertFormat,
                stalkTags,
                rc.Url,
                rc.Page,
                rc.User,
                rc.EditSummary,
                sizeDiff,
                rc.EditFlags,
                DateTime.UtcNow.ToString(this.appConfig.DateFormat)
            );
        }

        private string FormatMessageForEmail(IEnumerable<IStalk> stalks, IRecentChange rc, IBotUser botUser)
        {
            var stalksFormatted = this.FormatStalkListForEmail(stalks, botUser);

            var sizeDiff = "N/A";
            if (rc.SizeDiff.HasValue)
            {
                sizeDiff = (rc.SizeDiff.Value > 0 ? "+" : string.Empty) +
                           rc.SizeDiff.Value.ToString(CultureInfo.InvariantCulture);
            }

            return string.Format(
                this.templates.EmailRcTemplate,
                stalksFormatted,
                rc.Url,
                rc.Page,
                rc.User,
                rc.EditSummary,
                sizeDiff,
                rc.EditFlags,
                DateTime.UtcNow.ToString(this.appConfig.DateFormat)
            );
        }

        public string FormatStalkListForEmail(IEnumerable<IStalk> stalks, IBotUser botUser)
        {
            var stalkInfo = new StringBuilder();
            foreach (var stalk in stalks)
            {
                var expiry = stalk.ExpiryTime.HasValue
                    ? stalk.ExpiryTime.Value.ToString(this.appConfig.DateFormat)
                    : "never";

                var dynamicExpiry = stalk.DynamicExpiry.HasValue
                    ? " (on trigger, up to an additional " + stalk.DynamicExpiry.Value.ToString(this.appConfig.TimeSpanFormat) + ")"
                    : string.Empty;

                var lastTrigger = (stalk.LastTriggerTime.HasValue && stalk.LastTriggerTime != DateTime.MinValue)
                    ? stalk.LastTriggerTime.Value.ToString(this.appConfig.DateFormat)
                    : "never";

                var lastUpdate = stalk.LastUpdateTime.HasValue
                    ? stalk.LastUpdateTime.Value.ToString(this.appConfig.DateFormat)
                    : "never";

                var creation = stalk.CreationDate == DateTime.MinValue
                    ? stalk.CreationDate.ToString(this.appConfig.DateFormat)
                    : "before records began";

                var subList = new List<string>();
                var subItem = stalk.Subscribers.FirstOrDefault(x => x.Mask.ToString() == botUser.Mask.ToString());
                if (subItem != null)
                {
                    subList.Add(subItem.Subscribed ? "via stalk" : "excluded stalk");
                }

                if (this.channelConfig[stalk.Channel]
                    .Users
                    .Any(x => x.Mask.ToString() == botUser.Mask.ToString() && x.Subscribed))
                {
                    subList.Add("via channel");
                }

                var subscription = string.Join(", ", subList);
                if (string.IsNullOrWhiteSpace(subscription))
                {
                    subscription = "none";
                }

                stalkInfo.Append(
                    string.Format(
                        this.templates.EmailStalkTemplate,
                        stalk.Identifier,
                        stalk.Description,
                        stalk.SearchTree,
                        stalk.Channel,
                        expiry,
                        lastTrigger,
                        stalk.TriggerCount,
                        subscription,
                        lastUpdate,
                        stalk.WatchChannel,
                        dynamicExpiry,
                        creation
                    ));
            }

            var stalksFormatted = stalkInfo.ToString().TrimEnd();
            return stalksFormatted;
        }
    }
}