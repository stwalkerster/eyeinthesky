namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Services.Interfaces;
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
            IStalkConfiguration stalkConfig,
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

            if (this.StalkConfig.ContainsKey(stalkName))
            {
                throw new CommandErrorException("This stalk already exists!");
            }

            var stalkNode = this.CreateNode(stalkType, stalkTarget);

            var stalk = new ComplexStalk(stalkName)
            {
                SearchTree = stalkNode,
                IsEnabled = true,
                MailEnabled = true,
                ExpiryTime = DateTime.Now.AddMonths(3)
            };

            this.StalkConfig.Add(stalkName, stalk);
            
            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set {0} for new stalk {1} with CSL value: {2}",
                    stalkType,
                    stalkName,
                    stalkNode)
            };


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