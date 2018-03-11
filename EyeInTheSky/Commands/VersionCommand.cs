namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Reflection;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("version")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Standard)]
    public class VersionCommand : CommandBase
    {
        public VersionCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var ircVersion = Assembly.GetAssembly(typeof(IrcClient)).GetName().Version;
            var botLibVersion = Assembly.GetAssembly(typeof(CommandBase)).GetName().Version;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "EyeInTheSky v{0}, using Stwalkerster.IrcClient v{1}, Stwalkerster.Bot.CommandLib v{2}",
                    assemblyVersion,
                    ircVersion,
                    botLibVersion)
            };
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
            {
                {
                    string.Empty,
                    new HelpMessage(
                        this.CommandName,
                        string.Empty,
                        "Returns the current version of the running code")
                }
            };
        }
    }
}