namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Diagnostics;
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
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("version")]
    [CommandFlag(CLFlag.Standard)]
    public class VersionCommand : CommandBase
    {
        public VersionCommand(string commandSource,
            IUser user,
            IList<string> arguments,
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

        [Help("", "Returns the current version of the running code")]
        protected override IEnumerable<CommandResponse> Execute()
        {
            var assemblyVersion = this.GetFileVersion(Assembly.GetExecutingAssembly());
            var ircVersion = this.GetFileVersion(Assembly.GetAssembly(typeof(IrcClient)));
            var botLibVersion = this.GetFileVersion(Assembly.GetAssembly(typeof(CommandBase)));

            yield return new CommandResponse
            {
                Message = string.Format(
                    "EyeInTheSky v{0}, using Stwalkerster.IrcClient v{1}, Stwalkerster.Bot.CommandLib v{2}",
                    assemblyVersion,
                    ircVersion,
                    botLibVersion)
            };
        }

        private string GetFileVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }
    }
}