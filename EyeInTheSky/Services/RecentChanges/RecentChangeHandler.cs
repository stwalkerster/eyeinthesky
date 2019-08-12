namespace EyeInTheSky.Services.RecentChanges
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Email.Interfaces;
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
        private readonly IEmailTemplateFormatter emailTemplateFormatter;
        private readonly IStalkSubscriptionHelper subscriptionHelper;

        public RecentChangeHandler(
            IAppConfiguration appConfig,
            ILogger logger,
            IChannelConfiguration channelConfig,
            IBotUserConfiguration botUserConfiguration,
            IIrcClient freenodeClient,
            IEmailHelper emailHelper,
            INotificationTemplates templates,
            IEmailTemplateFormatter emailTemplateFormatter,
            IStalkSubscriptionHelper subscriptionHelper)
        {
            this.appConfig = appConfig;
            this.logger = logger;
            this.channelConfig = channelConfig;
            this.botUserConfiguration = botUserConfiguration;
            this.freenodeClient = freenodeClient;
            this.emailHelper = emailHelper;
            this.templates = templates;
            this.emailTemplateFormatter = emailTemplateFormatter;
            this.subscriptionHelper = subscriptionHelper;

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
                var channel = this.channelConfig[stalk.Channel];
                var userSubscriptionsToStalk = this.subscriptionHelper.GetUserSubscriptionsToStalk(channel, stalk);

                foreach (var subscription in userSubscriptionsToStalk.Where(x => x.IsSubscribed))
                {
                    stalkSplit[subscription.BotUser].Add(subscription.Stalk);
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
                    this.emailTemplateFormatter.FormatRecentChangeStalksForEmail(stalks, rc, botUser),
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
    }
}