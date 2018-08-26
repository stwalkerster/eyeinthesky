namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.BaseImplementations;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandFlag(Flag.Standard)]
    [CommandInvocation("help")]
    public class HelpCommand : HelpCommandBase
    {
        public HelpCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            ICommandParser commandParser,
            IIrcClient client) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            commandParser,
            client)
        {
        }
    }
}