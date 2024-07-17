namespace EyeInTheSky.Services.RecentChanges.Irc
{
    using System;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges.Interfaces;
    using EyeInTheSky.Services.RecentChanges.Irc.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Model;

    public class IrcRecentChangeHandler
    {
        private readonly ILogger logger;
        private readonly IAppConfiguration appConfig;
        private readonly IBugReporter bugReporter;
        private readonly IIrcRecentChangeParser rcParser;
        private readonly IRecentChangeHandler rcHandler;

        private IrcUserMask rcUserMaskCache;

        public IrcRecentChangeHandler(
            ILogger logger,
            IAppConfiguration appConfiguration,
            IIrcRecentChangeParser rcParser,
            IBugReporter bugReporter,
            IRecentChangeHandler rcHandler)
        {
            this.logger = logger;
            this.appConfig = appConfiguration;
            this.rcParser = rcParser;
            this.bugReporter = bugReporter;
            this.rcHandler = rcHandler;
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
                rc = this.rcParser.Parse(rcMessage, e.Target);
            }
            catch (LogParseException ex)
            {
                // `:NICK!USER@HOST COMMAND :MESSAGE` - 1 `@`, 1 `!`, 2 `:`, and 2 spaces = 6 
                var totalLength = e.User.Nickname.Length + e.User.Username.Length + e.User.Hostname.Length
                    + (e.IsNotice ? "NOTICE".Length : "PRIVMSG".Length) + e.Message.Length
                    + 6;
                ex.MessageLength = totalLength;
                
                this.bugReporter.ReportBug(ex);
                return;
            }
            catch (FormatException ex)
            {
                this.logger.ErrorFormat(ex, "Error processing received message: {0}", e.Message);
                return;
            }

            this.rcHandler.HandleRcEvent(e, rc);
        }
    }
}