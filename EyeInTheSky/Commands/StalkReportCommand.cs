namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("stalkreport")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class StalkReportCommand : CommandBase
    {
        private readonly StalkConfiguration stalkConfig;
        private readonly IEmailHelper emailHelper;
        private readonly RecentChangeHandler recentChangeHandler;
        private readonly INotificationTemplates templates;

        public StalkReportCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            StalkConfiguration stalkConfig,
            IEmailHelper emailHelper,
            RecentChangeHandler recentChangeHandler,
            INotificationTemplates templates) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.stalkConfig = stalkConfig;
            this.emailHelper = emailHelper;
            this.recentChangeHandler = recentChangeHandler;
            this.templates = templates;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var stalks = this.stalkConfig.StalkList;
            
            var disabled = stalks.Where(x => !x.IsEnabled);
            var expired = stalks.Where(x => x.ExpiryTime != null && x.ExpiryTime < DateTime.Now);            
            var active = stalks.Where(x => x.IsActive());
            
            var body = string.Format(
                this.templates.EmailStalkReport,
                this.recentChangeHandler.FormatStalkListForEmail(active),
                this.recentChangeHandler.FormatStalkListForEmail(disabled),
                this.recentChangeHandler.FormatStalkListForEmail(expired)
            );

            this.emailHelper.SendEmail(body, this.templates.EmailStalkReportSubject, null);

            yield return new CommandResponse
            {
                Destination = CommandResponseDestination.PrivateMessage,
                Type = CommandResponseType.Notice,
                Message = "Stalk report sent via email"
            };
        }
    }
}