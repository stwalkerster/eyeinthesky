namespace EyeInTheSky.Services.Email
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Email.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class EmailTemplateFormatter : IEmailTemplateFormatter
    {
        private readonly IAppConfiguration appConfig;
        private readonly IChannelConfiguration channelConfig;
        private readonly INotificationTemplates templates;

        public event EventHandler<StalkInfoFormattingEventArgs> OnStalkFormat;

        public EmailTemplateFormatter(
            IAppConfiguration appConfig,
            IChannelConfiguration channelConfig,
            INotificationTemplates templates)
        {
            this.appConfig = appConfig;
            this.channelConfig = channelConfig;
            this.templates = templates;
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

                var hookContent = "\n";

                var handler = this.OnStalkFormat;
                if (handler != null)
                {
                    var stalkInfoFormattingEventArgs = new StalkInfoFormattingEventArgs(stalk);
                    handler(this, stalkInfoFormattingEventArgs);
                    hookContent = stalkInfoFormattingEventArgs.ToString();
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
                        creation,
                        hookContent
                    ));
            }

            var stalksFormatted = stalkInfo.ToString().TrimEnd();
            return stalksFormatted;
        }

        public string FormatRecentChangeStalksForEmail(IEnumerable<IStalk> stalks, IRecentChange rc, IBotUser botUser)
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
    }
}