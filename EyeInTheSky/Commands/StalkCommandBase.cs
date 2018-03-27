namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public abstract class StalkCommandBase : CommandBase
    {
        protected StalkConfiguration StalkConfig { get; private set; }
        private IStalkNodeFactory StalkNodeFactory { get; set; }

        protected StalkCommandBase(string commandSource,
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
            this.StalkConfig = stalkConfig;
            this.StalkNodeFactory = stalkNodeFactory;
        }

        protected IStalkNode CreateNode(string type, string stalkTarget)
        {
            IStalkNode newNode;

            var escapedTarget = Regex.Escape(stalkTarget);
            
            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(escapedTarget);
                    newNode = usn;
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(escapedTarget);
                    newNode = psn;
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(escapedTarget);
                    newNode = ssn;
                    break;
                case "xml":
                    try
                    {
                        var xd = new XmlDocument();
                        xd.LoadXml(stalkTarget);

                        newNode = this.StalkNodeFactory.NewFromXmlFragment((XmlElement) xd.FirstChild);
                    }
                    catch (XmlException ex)
                    {
                        throw new CommandErrorException(ex.Message, ex);
                    }

                    break;
                default:
                    throw new CommandErrorException("Unknown stalk type!");
            }

            return newNode;
        }
    }
}