using EyeInTheSky.Helpers.Interfaces;

namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("quick")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class QuickCommand : CommandBase
    {
        private readonly StalkConfiguration stalkConfig;
        private readonly IStalkNodeFactory stalkNodeFactory;

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
            client)
        {
            this.stalkConfig = stalkConfig;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        #region Overrides of GenericCommand

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.Arguments.ToList();

            if (tokenList.Count < 3)
            {
                throw new ArgumentCountException(3, tokenList.Count);
            }

            var name = tokenList.PopFromFront();
            var type = tokenList.PopFromFront();
            var stalkTarget = string.Join(" ", tokenList);

            var s = new ComplexStalk(name);

            if (this.stalkConfig.Stalks.ContainsKey(name))
            {
                throw new CommandErrorException("This stalk already exists!");
            }

            this.stalkConfig.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(stalkTarget);
                    s.SearchTree = usn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for new stalk {1} with CSL value: {2}", type, name, usn)
                    };
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(stalkTarget);
                    s.SearchTree = psn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for new stalk {1} with CSL value: {2}", type, name, psn)
                    };
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(stalkTarget);
                    s.SearchTree = ssn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for new stalk {1} with CSL value: {2}", type, name, ssn)
                    };
                    break;
                case "xml":
                    var xmlfragment = stalkTarget;
                    IStalkNode node;
                    try
                    {
                        var xd = new XmlDocument();
                        xd.LoadXml(xmlfragment);

                        node = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) xd.FirstChild);
                        s.SearchTree = node;
                    }
                    catch (XmlException ex)
                    {
                        throw new CommandErrorException("XML error setting search tree", ex);
                    }

                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for new stalk {1} with CSL value: {2}", type, name, node)
                    };
                    break;
                default:
                    throw new CommandErrorException("Unknown stalk type");
            }

            s.IsEnabled = true;

            this.stalkConfig.Save();
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