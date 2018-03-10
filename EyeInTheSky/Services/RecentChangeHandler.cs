﻿namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class RecentChangeHandler
    {
        private readonly IAppConfiguration appConfig;
        private readonly ILogger logger;
        private readonly StalkConfiguration stalkConfig;
        private readonly IIrcClient freenodeClient;
        private readonly IRecentChangeParser rcParser;

        public RecentChangeHandler(IAppConfiguration appConfig, ILogger logger, StalkConfiguration stalkConfig, IIrcClient freenodeClient, IRecentChangeParser rcParser)
        {
            this.appConfig = appConfig;
            this.logger = logger;
            this.stalkConfig = stalkConfig;
            this.freenodeClient = freenodeClient;
            this.rcParser = rcParser;
        }

        public void OnReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Command != "PRIVMSG")
            {
                return;
            }

            try
            {
                var source = IrcUser.FromPrefix(e.Message.Prefix);

                if (!source.Equals(this.appConfig.RcUser))
                {
                    this.logger.WarnFormat(
                        "Received private message from {0} instead of expected {1}!",
                        source.ToString(),
                        this.appConfig.RcUser.ToString());
                    return;
                }

                var rcMessage = e.Message.Parameters.Skip(1).FirstOrDefault();
                var rc = this.rcParser.Parse(rcMessage);

                var stalks = this.stalkConfig.Stalks.Search(rc).ToList();

                if (stalks.Count == 0)
                {
                    return;
                }
                
                this.logger.InfoFormat("Seen stalked change for stalks: {0}", stalks.Select(x => x.Flag).Implode());
                
                // touch update flag
                foreach (var stalk in stalks)
                {
                    stalk.LastTriggerTime = DateTime.Now;
                }
                
                // send notifications
                this.SendToIrc(stalks, rc);
                this.SendEmail(stalks, rc);
            }
            catch (Exception ex)
            {
                this.logger.ErrorFormat(ex, "Error processing received message: {0}", e.Message);
            }
        }

        private void SendEmail(IList<IStalk> stalks, IRecentChange rc)
        {
            if (!stalks.Any(x => x.MailEnabled))
            {
                this.logger.Info("Not sending email; no matched stalks support it.");
                return;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(this.appConfig.EmailConfiguration.Sender),
                Subject = string.Format(this.appConfig.EmailConfiguration.Subject, stalks.Select(x => x.Flag).Implode(", ")),
                Body = this.FormatMessageForEmail(stalks, rc)
            };

            mailMessage.To.Add(this.appConfig.EmailConfiguration.To);

            var client = new SmtpClient(this.appConfig.EmailConfiguration.Hostname, this.appConfig.EmailConfiguration.Port);

            client.Send(mailMessage);
        }

        private void SendToIrc(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            this.freenodeClient.SendMessage(this.appConfig.FreenodeChannel, this.FormatMessageForIrc(stalks, rc));
        }

        public string FormatMessageForIrc(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            var stalkTags = new StringBuilder();
            bool first = true;
            foreach (var s in stalks)
            {
                if (!first)
                {
                    stalkTags.Append(this.appConfig.IrcStalkTagSeparator);
                }
                
                first = false;

                stalkTags.Append(s.Flag);
            }

            return string.Format(
                this.appConfig.IrcAlertFormat,
                stalkTags,
                rc.Url,
                rc.Page,
                rc.User,
                rc.EditSummary,
                (rc.SizeDifference > 0 ? "+" : string.Empty) + rc.SizeDifference.ToString(CultureInfo.InvariantCulture),
                rc.EditFlags,
                DateTime.Now.ToString(this.appConfig.DateFormat)
            );
        }

        public string FormatMessageForEmail(IEnumerable<IStalk> stalks, IRecentChange rc)
        {
            var stalkInfo = new StringBuilder();
            foreach (var stalk in stalks)
            {
                stalkInfo.Append(
                    string.Format(
                        this.appConfig.EmailStalkTemplate,
                        stalk.Flag,
                        stalk.Description,
                        stalk.SearchTree,
                        stalk.MailEnabled,
                        stalk.ExpiryTime.ToString(this.appConfig.DateFormat)
                    ));
            }

            return string.Format(
                this.appConfig.EmailRcTemplate,
                stalkInfo,
                rc.Url,
                rc.Page,
                rc.User,
                rc.EditSummary,
                (rc.SizeDifference > 0 ? "+" : string.Empty) + rc.SizeDifference.ToString(CultureInfo.InvariantCulture),
                rc.EditFlags,
                DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString()
            );
        }
    }
}