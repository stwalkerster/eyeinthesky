namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class RecentChangeHandler
    {
        private readonly IAppConfiguration appConfig;
        private readonly ILogger logger;
        private readonly IStalkConfiguration stalkConfig;
        private readonly IIrcClient freenodeClient;
        private readonly IRecentChangeParser rcParser;
        private readonly IEmailHelper emailHelper;
        private readonly INotificationTemplates templates;
        private readonly IBugReporter bugReporter;

        private IrcUserMask rcUserMaskCache;

        public RecentChangeHandler(IAppConfiguration appConfig,
            ILogger logger,
            IStalkConfiguration stalkConfig,
            IIrcClient freenodeClient,
            IRecentChangeParser rcParser,
            IEmailHelper emailHelper,
            INotificationTemplates templates,
            IBugReporter bugReporter)
        {
            this.appConfig = appConfig;
            this.logger = logger;
            this.stalkConfig = stalkConfig;
            this.freenodeClient = freenodeClient;
            this.rcParser = rcParser;
            this.emailHelper = emailHelper;
            this.templates = templates;
            this.bugReporter = bugReporter;

            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Warn("Not sending email; email configuration is disabled");
            }
        }

        public void OnReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (e.IsNotice)
            {
                return;
            }

            var source = e.User;

            // first check is performance-related, second is thread safety related.
            if (this.rcUserMaskCache == null)
            {
                lock (this)
                {
                    try
                    {
                        if (this.rcUserMaskCache == null)
                        {
                            this.rcUserMaskCache = new IrcUserMask(this.appConfig.RcUser, e.Client);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.ErrorFormat(ex, "Encountered error building mask, on receipt of {0}", e.Message);
                        return;
                    }
                }
            }

            if (!this.rcUserMaskCache.Matches(source).GetValueOrDefault(false))
            {
                this.logger.WarnFormat(
                    "Received private message from {0} instead of expected {1}!",
                    source.ToString(),
                    this.appConfig.RcUser);
                return;
            }
            
            var rcMessage = e.Message;
            
            
            IRecentChange rc;

            try
            {
                rc = this.rcParser.Parse(rcMessage);
            }
            catch (BugException ex)
            {
                this.bugReporter.ReportBug(ex);
                return;
            }
            catch (FormatException ex)
            {
                this.logger.ErrorFormat(ex, "Error processing received message: {0}", e.Message);
                return;
            }
            
            var stalks = this.stalkConfig.MatchStalks(rc).ToList();

            if (stalks.Count == 0)
            {
                return;
            }

            this.logger.InfoFormat("Seen stalked change for stalks: {0}", string.Join(" ", stalks.Select(x => x.Flag)));
            
            // send notifications
            this.SendToIrc(stalks, rc);
            this.SendEmail(stalks, rc);
                
            // touch update flag
            foreach (var stalk in stalks)
            {
                stalk.LastTriggerTime = DateTime.Now;
                stalk.TriggerCount++;
            }
            
            this.stalkConfig.Save();
        }

        private void SendEmail(IList<IStalk> stalks, IRecentChange rc)
        {
            if (!stalks.Any(x => x.MailEnabled))
            {
                this.logger.Debug("Not sending email; no matched stalks support it.");
                return;
            }
            
            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Debug("Not sending email; email configuration is disabled");
                return;
            }

            var stalkList = string.Join(", ", stalks.Select(x => x.Flag));
            var messageId = stalks.Count(x => x.LastMessageId != null) > 1 ? null : stalks.First().LastMessageId;

            try
            {
                messageId = this.emailHelper.SendEmail(
                    this.FormatMessageForEmail(stalks, rc),
                    string.Format(this.templates.EmailRcSubject, stalkList, rc.Page),
                    messageId);
            }
            catch (Exception ex)
            {
                this.logger.ErrorFormat(ex, "Failed to send notification email for RC {0}", rc);
            }

            foreach (var stalk in stalks)
            {
                stalk.LastMessageId = messageId;
            }
        }

        private void SendToIrc(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            try
            {
                this.freenodeClient.SendMessage(this.appConfig.FreenodeChannel, this.FormatMessageForIrc(stalks, rc));
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

                stalkTags.Append(s.Flag);
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
                DateTime.Now.ToString(this.appConfig.DateFormat)
            );
        }

        public string FormatMessageForEmail(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            var stalksFormatted = this.FormatStalkListForEmail(stalks);

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
                DateTime.Now.ToString(this.appConfig.DateFormat)
            );
        }

        public string FormatStalkListForEmail(IEnumerable<IStalk> stalks)
        {
            var stalkInfo = new StringBuilder();
            foreach (var stalk in stalks)
            {
                var expiry = stalk.ExpiryTime.HasValue
                    ? stalk.ExpiryTime.Value.ToString(this.appConfig.DateFormat)
                    : "never";
                
                var lastTrigger = stalk.LastTriggerTime.HasValue
                    ? stalk.LastTriggerTime.Value.ToString(this.appConfig.DateFormat)
                    : "never";

                stalkInfo.Append(
                    string.Format(
                        this.templates.EmailStalkTemplate,
                        stalk.Flag,
                        stalk.Description,
                        stalk.SearchTree,
                        stalk.MailEnabled,
                        expiry,
                        lastTrigger,
                        stalk.TriggerCount
                    ));
            }

            var stalksFormatted = stalkInfo.ToString().TrimEnd();
            return stalksFormatted;
        }
    }
}