using EyeInTheSky.Helpers.Interfaces;

namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("quick")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class QuickCommand : StalkCommandBase
    {
        public QuickCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            StalkConfiguration stalkConfig,
            IStalkNodeFactory stalkNodeFactory) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client,
            stalkConfig,
            stalkNodeFactory)
        {
        }

        #region Overrides of GenericCommand

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.Arguments.ToList();

            if (tokenList.Count < 3)
            {
                throw new ArgumentCountException(3, tokenList.Count);
            }

            var stalkName = tokenList.PopFromFront();
            var stalkType = tokenList.PopFromFront();
            var stalkTarget = string.Join(" ", tokenList);

            var stalk = new ComplexStalk(stalkName);

            if (this.StalkConfig.Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException("This stalk already exists!");
            }

            this.StalkConfig.Stalks.Add(stalkName, stalk);

            var stalkNode = this.CreateNode(stalkType, stalkTarget);
            stalk.SearchTree = stalkNode;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set {0} for new stalk {1} with CSL value: {2}",
                    stalkType,
                    stalkName,
                    stalkNode)
            };

            stalk.IsEnabled = true;

            this.StalkConfig.Save();
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
            {
                {
                    string.Empty,
                    new HelpMessage(
                        this.CommandName,
                        "<Flag> <Type> <Match>",
                        "Sets up a new basic stalk with the specified match and type, and default options specified.")
                }
            };
        }

        #endregion
    }
}