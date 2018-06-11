﻿namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
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
        private readonly IAppConfiguration config;
        private readonly INotificationTemplates templates;
        private readonly IEmailHelper emailHelper;
        private readonly RecentChangeHandler recentChangeHandler;
        private readonly IXmlCacheService xmlCacheService;
        private readonly IStalkNodeFactory stalkNodeFactory;
        private readonly IStalkConfiguration stalkConfig;

        public StalkCommand(
            string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IStalkConfiguration stalkConfig,
            IStalkNodeFactory stalkNodeFactory,
            IAppConfiguration config,
            INotificationTemplates templates,
            IEmailHelper emailHelper,
            RecentChangeHandler recentChangeHandler,
            IXmlCacheService xmlCacheService
        ) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.config = config;
            this.templates = templates;
            this.emailHelper = emailHelper;
            this.recentChangeHandler = recentChangeHandler;
            this.xmlCacheService = xmlCacheService;
            this.stalkNodeFactory = stalkNodeFactory;
            this.stalkConfig = stalkConfig;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();
            
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(1, this.Arguments.Count());
            }

            string mode = tokenList.PopFromFront();

            switch (mode)
            {
                case "add":
                    return this.AddMode(tokenList);
                case "list":
                    return this.ListMode();
                case "report":
                    return this.ReportMode();
            }

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count(), mode);
            }
            
            var stalkName = tokenList.PopFromFront();
            if (!this.stalkConfig.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }
            
            switch (mode)
            {
                case "del":
                    return this.DeleteMode(stalkName);
                case "export":
                    return this.ExportMode(stalkName);
                case "set":
                    return this.SetMode(tokenList, stalkName);
                case "mail":
                    return this.MailMode(tokenList, stalkName);
                case "description":
                    return this.DescriptionMode(tokenList, stalkName);
                case "expiry":
                    return this.ExpiryMode(tokenList, stalkName);
                case "enabled":
                    return this.EnabledMode(tokenList, stalkName);
                case "and":
                    return this.AndMode(tokenList, stalkName);
                case "or":
                    return this.OrMode(tokenList, stalkName);
                // Aliases:
                case "enable":
                    return this.EnabledMode(new List<string> {"true"}, stalkName);
                case "disable":
                    return this.EnabledMode(new List<string> {"false"}, stalkName);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> ReportMode()
        {
            var stalks = this.stalkConfig.Items;
            
            var disabled = stalks.Where(x => !x.IsEnabled);
            var expired = stalks.Where(x => x.ExpiryTime != null && x.ExpiryTime < DateTime.Now);            
            var active = stalks.Where(x => x.IsActive());
            
            var body = string.Format(
                this.templates.EmailStalkReport,
                this.recentChangeHandler.FormatStalkListForEmail(active),
                this.recentChangeHandler.FormatStalkListForEmail(disabled),
                this.recentChangeHandler.FormatStalkListForEmail(expired)
            );

            this.emailHelper.SendEmail(body, this.templates.EmailStalkReportSubject, null);

            yield return new CommandResponse
            {
                Destination = CommandResponseDestination.PrivateMessage,
                Type = CommandResponseType.Notice,
                Message = "Stalk report sent via email"
            };
        }

        private IEnumerable<CommandResponse> OrMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "or");
            }

            var newStalkType = tokenList.PopFromFront();
            
            var stalk = this.stalkConfig[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newNode = this.CreateNode(newStalkType, newTarget);
            if (stalk.SearchTree.GetType() == typeof(OrNode))
            {
                ((OrNode)stalk.SearchTree).ChildNodes.Add(newNode);
            }
            else
            {
                var newroot = new OrNode
                {
                    ChildNodes = new List<IStalkNode>
                    {
                        stalk.SearchTree,
                        newNode
                    }
                };
                
                stalk.SearchTree = newroot;
            }

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set {0} for stalk {1} with CSL value: {2}",
                    newStalkType,
                    stalkName,
                    stalk.SearchTree)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> AndMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "and");
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.stalkConfig[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newNode = this.CreateNode(newStalkType, newTarget);
            
            if (stalk.SearchTree.GetType() == typeof(AndNode))
            {
                ((AndNode)stalk.SearchTree).ChildNodes.Add(newNode);
            }
            else
            {
                var newroot = new AndNode
                {
                    ChildNodes = new List<IStalkNode>
                    {
                        stalk.SearchTree,
                        newNode
                    }
                };
                
                stalk.SearchTree = newroot;
            }

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set {0} for stalk {1} with CSL value: {2}",
                    newStalkType,
                    stalkName,
                    stalk.SearchTree)
            };

            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> EnabledMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "enabled");
            }
            
            bool enabled;
            var possibleBoolean = tokenList.PopFromFront();
            if (!BooleanParser.TryParse(possibleBoolean, out enabled))
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
            
            this.stalkConfig[stalkName].IsEnabled = enabled;
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ExpiryMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "expiry");
            }

            var date = string.Join(" ", tokenList);

            var expiryTime = DateTime.Parse(date);
            this.stalkConfig[stalkName].ExpiryTime = expiryTime;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set expiry attribute on stalk {0} to {1}",
                    stalkName,
                    expiryTime.ToString(this.config.DateFormat))
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DescriptionMode(List<string> tokenList, string stalkName)
        {
            var descr = string.Join(" ", tokenList);

            if (string.IsNullOrWhiteSpace(descr))
            {
                this.stalkConfig[stalkName].Description = null;
                
                yield return new CommandResponse
                {
                    Message = string.Format("Cleared description attribute on stalk {0}", stalkName)
                };
            }
            else
            {
                this.stalkConfig[stalkName].Description = descr;

                yield return new CommandResponse
                {
                    Message = string.Format("Set description attribute on stalk {0} to {1}", stalkName, descr)
                };
            }

            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> MailMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "mail");
            }

            bool mail;
            var possibleBoolean = tokenList.PopFromFront();
            if (!BooleanParser.TryParse(possibleBoolean, out mail))
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
            
            this.stalkConfig[stalkName].MailEnabled = mail;
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ListMode()
        {
            var activeStalks = this.stalkConfig.Items.Where(x => x.IsActive()).ToList();

            if (!activeStalks.Any())
            {
                yield return new CommandResponse
                {
                    Message = "There are no active stalks.",
                    Type = CommandResponseType.Notice,
                    Destination = CommandResponseDestination.PrivateMessage
                };
                
                yield break;
            }
            
            yield return new CommandResponse
            {
                Message = "Active stalk list:",
                Type = CommandResponseType.Notice,
                Destination = CommandResponseDestination.PrivateMessage
            };

            
            foreach (var stalk in activeStalks)
            {
                var message = string.Format(
                    "Flag: {0}, Last modified: {1}, Type: Complex {2}",
                    stalk.Flag,
                    stalk.LastUpdateTime.GetValueOrDefault().ToString(this.config.DateFormat),
                    stalk.SearchTree);
                
                yield return new CommandResponse
                {
                    Message = message,
                    Type = CommandResponseType.Notice,
                    Destination = CommandResponseDestination.PrivateMessage
                };
            }

            yield return new CommandResponse
            {
                Message = "End of stalk list.",
                Type = CommandResponseType.Notice,
                Destination = CommandResponseDestination.PrivateMessage
            };

            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> SetMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "set");
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.stalkConfig[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newroot = this.CreateNode(newStalkType, newTarget);
            stalk.SearchTree = newroot;

            yield return new CommandResponse
            {
                Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", newStalkType, stalkName, newroot)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DeleteMode(string stalkName)
        {
            this.stalkConfig.Remove(stalkName);
            
            yield return new CommandResponse
            {
                Message = string.Format("Deleted stalk {0}", stalkName)
            };
            
            this.stalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ExportMode(string stalkName)
        {
            var searchTree = this.stalkConfig[stalkName].SearchTree;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "{0}: {1}",
                    stalkName,
                    this.stalkNodeFactory.ToXml(new XmlDocument(), searchTree).OuterXml
                )
            };
        }

        private IEnumerable<CommandResponse> AddMode(List<string> tokenList)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count(), "add");
            }

            var stalkName = tokenList.First();
            var stalk = new ComplexStalk(stalkName);

            this.stalkConfig.Add(stalkName, stalk);
            
            yield return new CommandResponse
            {
                Message = string.Format("Added disabled stalk {0} with CSL value: {1}", stalkName, stalk.SearchTree)
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
                        "Lists all active stalks")
                },{
                    "add",
                    new HelpMessage(
                        this.CommandName,
                        "add <Flag>",
                        "Adds a new unconfigured stalk")
                },{
                    "report",
                    new HelpMessage(
                        this.CommandName,
                        "report",
                        "Sends a report on the status of all stalks via email")
                },{
                    "del",
                    new HelpMessage(
                        this.CommandName,
                        "del <Flag>",
                        "Deletes a stalk")
                },{
                    "export",
                    new HelpMessage(
                        this.CommandName,
                        "export <Flag>",
                        "Retrieves the XML search tree of a specific stalk")
                },{
                    "enabled",
                    new HelpMessage(
                        this.CommandName,
                        new[]{"enabled <Flag> <true|false>", "enable <Flag>", "disable <Flag>"},
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
                        "Sets the stalk configuration of the specified stalk to specified user, page, or edit summary. Alternatively, manually specify an XML tree (advanced).")
                },{
                    "and",
                    new HelpMessage(
                        this.CommandName,
                        "and <Flag> <user|page|summary|xml> <Match...>",
                        "Sets the stalk configuration of the specified stalk to the logical AND of the current configuration, and a specified user, page, or edit summary; or XML tree (advanced).")
                },{
                    "or",
                    new HelpMessage(
                        this.CommandName,
                        "or <Flag> <user|page|summary|xml> <Match...>",
                        "Sets the stalk configuration of the specified stalk to the logical OR of the current configuration, and a specified user, page, or edit summary; or XML tree (advanced).")
                },
            };
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
                        var xml = this.xmlCacheService.RetrieveXml(this.User);
                        if (xml == null)
                        {
                            throw new CommandErrorException("No cached XML. Please use the xml command first.");
                        }
                        
                        newNode = this.stalkNodeFactory.NewFromXmlFragment(xml);
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
