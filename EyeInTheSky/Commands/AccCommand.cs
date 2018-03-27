namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using EyeInTheSky.Model;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("acc")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class AccCommand : CommandBase
    {
        private readonly IStalkConfiguration stalkConfig;

        public AccCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IStalkConfiguration stalkConfig)
            : base(
                commandSource,
                user,
                arguments,
                logger,
                flagService,
                configurationProvider,
                client)
        {
            this.stalkConfig = stalkConfig;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.Arguments.ToList();

            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(2, tokenList.Count);
            }

            var accRequestId = tokenList.PopFromFront();
            var username = string.Join(" ", tokenList);
            var regexData = Regex.Escape(username);
            
            var userStalkNode = new UserStalkNode();
            var pageStalkNode = new PageStalkNode();
            var summaryStalkNode = new SummaryStalkNode();

            userStalkNode.SetMatchExpression(regexData);
            pageStalkNode.SetMatchExpression(regexData);
            summaryStalkNode.SetMatchExpression(regexData);

            var rootNode = new OrNode
            {
                ChildNodes = new List<IStalkNode> {userStalkNode, pageStalkNode, summaryStalkNode}
            };

            var stalk = new ComplexStalk("acc" + accRequestId)
            {
                MailEnabled = true,
                Description = "ACC " + accRequestId + ": " + username,
                SearchTree = rootNode,
                IsEnabled = true,
                ExpiryTime = DateTime.Now.AddMonths(3)
            };

            this.stalkConfig.Add("acc" + accRequestId, stalk);
            this.stalkConfig.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Set new stalk {0} with CSL value: {1}", stalk.Flag, stalk.SearchTree)
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
                        "<ID> <Username>",
                        "Sets up a new temporary stalk based on an ACC request")
                }
            };
        }
    }
}