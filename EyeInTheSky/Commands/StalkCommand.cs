using EyeInTheSky.Helpers.Interfaces;

namespace EyeInTheSky.Commands
{
    using System;
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

    [CommandInvocation("stalk")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class StalkCommand : CommandBase
    {
        private readonly StalkConfiguration stalkConfig;
        private readonly IStalkNodeFactory stalkNodeFactory;

        public StalkCommand(string commandSource,
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

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.Arguments.ToList();
            
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(1, this.Arguments.Count());
            }

            string mode = tokenList.PopFromFront();
            
            switch (mode)
            {
                case "add":
                    return this.AddMode(tokenList);
                case "del":
                    return this.DeleteMode(tokenList);
                case "set":
                    return this.SetMode(tokenList);
                case "list":
                    return this.ListMode();
                case "mail":
                    return this.MailMode(tokenList);
                case "description":
                    return this.DescriptionMode(tokenList);
                case "expiry":
                    return this.ExpiryMode(tokenList);
                case "enabled":
                    return this.EnabledMode(tokenList);
                case "and":
                    return this.AndMode(tokenList);
                case "or":
                    return this.OrMode(tokenList);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> OrMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            string stalk = tokenList.PopFromFront();

            var s = this.stalkConfig.Stalks[stalk];

            string type = tokenList.PopFromFront();

            string stalkTarget = string.Join(" ", tokenList);

            var newroot = new OrNode {LeftChildNode = s.SearchTree};

            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = usn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = psn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = ssn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "xml":
                    string xmlfragment = stalkTarget;
                    try
                    {
                        var xd = new XmlDocument();
                        xd.LoadXml(xmlfragment);

                        var node = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) xd.FirstChild);

                        newroot.RightChildNode = node;
                        s.SearchTree = newroot;
                    }
                    catch (XmlException ex)
                    {
                        throw new CommandErrorException("XML error setting search tree", ex);
                    }
                    
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };

                    break;
                default:
                    throw new CommandErrorException("Unknown stalk type!");
            }
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> AndMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            string stalk = tokenList.PopFromFront();

            var s = this.stalkConfig.Stalks[stalk];

            string type = tokenList.PopFromFront();

            string stalkTarget = string.Join(" ", tokenList);

            var newroot = new AndNode {LeftChildNode = s.SearchTree};

            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = usn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = psn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(stalkTarget);
                    newroot.RightChildNode = ssn;
                    s.SearchTree = newroot;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };
                    break;
                case "xml":
                    string xmlfragment = stalkTarget;
                    try
                    {
                        var xd = new XmlDocument();
                        xd.LoadXml(xmlfragment);

                        var node = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) xd.FirstChild);

                        newroot.RightChildNode = node;
                        s.SearchTree = newroot;
                    }
                    catch (XmlException ex)
                    {
                        throw new CommandErrorException("XML error setting search tree", ex);
                    }
                    
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, newroot)
                    };

                    break;
                default:
                    throw new CommandErrorException("Unknown stalk type!");
            }
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> EnabledMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            var stalkName = tokenList.PopFromFront();
            
            bool enabled;
            var possibleBoolean = tokenList.PopFromFront();
            if (!bool.TryParse(possibleBoolean, out enabled))
            {
                throw new CommandErrorException(
                    string.Format(
                        "{0} is not a value of boolean I recognise. Try 'true', 'false' or ERR_FILE_NOT_FOUND.",
                        possibleBoolean));
            }
            
            yield return new CommandResponse
            {
                Message = string.Format("Set enabled attribute on stalk {0} to {1}", stalkName, enabled)
            };
            
            this.stalkConfig.Stalks[stalkName].IsEnabled = enabled;
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ExpiryMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            var stalk = tokenList.PopFromFront();
            var date = string.Join(" ", tokenList);

            var expiryTime = DateTime.Parse(date);
            this.stalkConfig.Stalks[stalk].ExpiryTime = expiryTime;
            this.Client.SendMessage(this.CommandSource, "Set expiry attribute on stalk " + stalk + " to " + expiryTime);
            
            yield return new CommandResponse
            {
                Message = string.Format("Set expiry attribute on stalk {0} to {1}", stalk, expiryTime)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DescriptionMode(List<string> tokenList)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count());
            }

            string stalk = tokenList.PopFromFront();
            string descr = string.Join(" ", tokenList);

            this.stalkConfig.Stalks[stalk].Description = descr;
            
            yield return new CommandResponse
            {
                Message = string.Format("Set description attribute on stalk {0} to {1}", stalk, descr)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> MailMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            var stalkName = tokenList.PopFromFront();

            bool mail;
            var possibleBoolean = tokenList.PopFromFront();
            if (!bool.TryParse(possibleBoolean, out mail))
            {
                throw new CommandErrorException(
                    string.Format(
                        "{0} is not a value of boolean I recognise. Try 'true', 'false' or ERR_FILE_NOT_FOUND.",
                        possibleBoolean));
            }

            yield return new CommandResponse
            {
                Message = string.Format("Set immediatemail attribute on stalk {0} to {1}", stalkName, mail)
            };
            
            this.stalkConfig.Stalks[stalkName].MailEnabled = mail;
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ListMode()
        {
            yield return new CommandResponse
            {
                Message = "Stalk list:",
                Type = CommandResponseType.Notice,
                Destination = CommandResponseDestination.PrivateMessage
            };
            
            foreach (var kvp in this.stalkConfig.Stalks)
            {
                yield return new CommandResponse
                {
                    Message = kvp.Value.ToString(),
                    Type = CommandResponseType.Notice,
                    Destination = CommandResponseDestination.PrivateMessage
                };
            }

            yield return new CommandResponse
            {
                Message = "End of stalk list:",
                Type = CommandResponseType.Notice,
                Destination = CommandResponseDestination.PrivateMessage
            };

            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> SetMode(List<string> tokenList)
        {
            if (tokenList.Count < 2)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            var stalk = tokenList.PopFromFront();

            if (!this.stalkConfig.Stalks.ContainsKey(stalk))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalk));
            }

            var s = this.stalkConfig.Stalks[stalk];

            var type = tokenList.PopFromFront();
            var regex = string.Join(" ", tokenList);

            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(regex);
                    s.SearchTree = usn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, usn)
                    };
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(regex);
                    s.SearchTree = psn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, psn)
                    };
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(regex);
                    s.SearchTree = ssn;
                    yield return new CommandResponse
                    {
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, ssn)
                    };
                    break;
                case "xml":
                    var xmlfragment = string.Join(" ", tokenList);
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
                        Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", type, stalk, node)
                    };
                    break;
                default:
                    throw new CommandErrorException("Unknown stalk type!");
            }
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DeleteMode(List<string> tokenList)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count());
            }

            var stalkName = tokenList.First();

            this.stalkConfig.Stalks.Remove(stalkName);
            
            yield return new CommandResponse
            {
                Message = string.Format("Deleted stalk {0}", stalkName)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> AddMode(List<string> tokenList)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count());
            }

            var stalkName = tokenList.First();
            var s = new ComplexStalk(stalkName);

            this.stalkConfig.Stalks.Add(stalkName, s);
            
            yield return new CommandResponse
            {
                Message = string.Format("Added stalk {0} with CSL value: {1}", stalkName, s.SearchTree)
            };
            
            this.stalkConfig.Save();
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
            {
                {
                    "list",
                    new HelpMessage(
                        this.CommandName,
                        "list",
                        "Lists all configured stalks")
                },{
                    "add",
                    new HelpMessage(
                        this.CommandName,
                        "add <Flag>",
                        "Adds a new unconfigured stalk")
                },{
                    "del",
                    new HelpMessage(
                        this.CommandName,
                        "del <Flag>",
                        "Deletes a stalk")
                },{
                    "enabled",
                    new HelpMessage(
                        this.CommandName,
                        "enabled <Flag> <true|false>",
                        "Marks a stalk as enabled or disabled")
                },{
                    "mail",
                    new HelpMessage(
                        this.CommandName,
                        "mail <Flag> <true|false>",
                        "Enables or disables email notifications for each trigger of the specified stalk")
                },{
                    "description",
                    new HelpMessage(
                        this.CommandName,
                        "description <Flag> <Description...>",
                        "Sets the description of the specified stalk")
                },{
                    "expiry",
                    new HelpMessage(
                        this.CommandName,
                        "expiry <Flag> <Description...>",
                        "Sets the description of the specified stalk")
                },{
                    "set",
                    new HelpMessage(
                        this.CommandName,
                        "set <Flag> <user|page|summary|xml> <Match...>",
                        "Sets the stalk configuration of the specified stalk to specified user, page, or edit summary regex. Alternatively, manually specify an XML tree (advanced).")
                },{
                    "and",
                    new HelpMessage(
                        this.CommandName,
                        "and <Flag> <user|page|summary|xml> <Match...>",
                        "Sets the stalk configuration of the specified stalk to the logical AND of the current configuration, and a specified user, page, or edit summary regex; or XML tree (advanced).")
                },{
                    "or",
                    new HelpMessage(
                        this.CommandName,
                        "or <Flag> <user|page|summary|xml> <Match...>",
                        "Sets the stalk configuration of the specified stalk to the logical OR of the current configuration, and a specified user, page, or edit summary regex; or XML tree (advanced).")
                },
            };
        }
    }
}
