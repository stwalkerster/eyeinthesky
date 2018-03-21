namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
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
        private readonly StalkConfiguration stalkConfig;

        public AccCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            StalkConfiguration stalkConfig)
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

            var id = tokenList.PopFromFront();
            var user = string.Join(" ", tokenList);

            var s = new ComplexStalk("acc" + id);

            var or = new OrNode();

            var uor = new OrNode();
            var usn = new UserStalkNode();
            usn.SetMatchExpression(user);

            var psn = new PageStalkNode();
            psn.SetMatchExpression(user);

            uor.LeftChildNode = usn;
            uor.RightChildNode = psn;

            var upor = new OrNode();

            var upsn = new PageStalkNode();
            upsn.SetMatchExpression("User:" + user);

            var utpsn = new PageStalkNode();
            utpsn.SetMatchExpression("User talk:" + user);

            upor.LeftChildNode = upsn;
            upor.RightChildNode = utpsn;

            var ssn = new SummaryStalkNode();
            ssn.SetMatchExpression(user);

            var or2 = new OrNode();
            or2.LeftChildNode = uor;
            or2.RightChildNode = upor;

            or.LeftChildNode = or2;
            or.RightChildNode = ssn;
            s.MailEnabled = true;
            s.Description = "ACC " + id + ": " + user;
            s.SearchTree = or;
            s.IsEnabled = true;

            s.ExpiryTime = DateTime.Now.AddMonths(3);

            this.stalkConfig.Stalks.Add("acc" + id, s);
            this.stalkConfig.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Set new stalk {0} with CSL value: {1}", s.Flag, or)
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