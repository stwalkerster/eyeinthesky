namespace EyeInTheSky.Commands
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
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("stalk")]
    [CommandFlag(CLFlag.Standard)]
    public class StalkCommand : CommandBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IAppConfiguration config;
        private readonly INotificationTemplates templates;
        private readonly IEmailHelper emailHelper;
        private readonly RecentChangeHandler recentChangeHandler;
        private readonly IXmlCacheService xmlCacheService;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IStalkNodeFactory stalkNodeFactory;

        public StalkCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IChannelConfiguration channelConfiguration,
            IStalkNodeFactory stalkNodeFactory,
            IAppConfiguration config,
            INotificationTemplates templates,
            IEmailHelper emailHelper,
            RecentChangeHandler recentChangeHandler,
            IXmlCacheService xmlCacheService,
            IBotUserConfiguration botUserConfiguration
        ) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.channelConfiguration = channelConfiguration;
            this.config = config;
            this.templates = templates;
            this.emailHelper = emailHelper;
            this.recentChangeHandler = recentChangeHandler;
            this.xmlCacheService = xmlCacheService;
            this.botUserConfiguration = botUserConfiguration;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        protected override void OnPreRun()
        {
            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
            }
        }

        [SubcommandInvocation("unsubscribe")]
        [CommandFlag(CLFlag.Standard)]
        [RequiredArguments(1)]
        [Help("<Identifier>", "Unsubscribes from email notifications for this stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> UnsubscribeMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var accountKey = string.Format("$a:{0}", this.User.Account);
            var botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                yield return new CommandResponse
                {
                    Message = "You must be a registered user to use this command."
                };

                yield break;
            }

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];

            if (stalk.Subscribers.All(x => x.Mask.ToString() != botUser.Mask.ToString()))
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are not subscribed to notifications for stalk {0}", stalkName)
                };

                yield break;
            }

            var item = stalk.Subscribers.FirstOrDefault(x => x.Mask.ToString() == botUser.Mask.ToString());
            stalk.Subscribers.Remove(item);
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Unsubscribed from notifications for stalk {0}", stalkName)
            };
        }

        [SubcommandInvocation("subscribe")]
        [CommandFlag(CLFlag.Standard)]
        [RequiredArguments(1)]
        [Help("<Identifier>", "Subscribes to email notifications for this stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> SubscribeMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var accountKey = string.Format("$a:{0}", this.User.Account);
            var botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                yield return new CommandResponse
                {
                    Message = "You must be a registered user with a confirmed email address to use this command."
                };

                yield break;
            }

            if (!botUser.EmailAddressConfirmed)
            {
                yield return new CommandResponse
                {
                    Message = "You must have a confirmed email address to use this command."
                };

                yield break;
            }

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];

            if (stalk.Subscribers.Any(x => x.Mask == botUser.Mask))
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are already subscribed to notifications for stalk {0}", stalkName)
                };

                yield break;
            }

            stalk.Subscribers.Add(new StalkUser(botUser.Mask));
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Subscribed to notifications for stalk {0}", stalkName)
            };
        }

        [SubcommandInvocation("report")]
        [CommandFlag(CLFlag.Standard)]
        [Help("", "Sends a report on the status of all stalks via email")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ReportMode()
        {
            var accountKey = string.Format("$a:{0}", this.User.Account);
            var botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                yield return new CommandResponse
                {
                    Message = "You must be a registered user with a confirmed email address to use this command."
                };

                yield break;
            }

            if (!botUser.EmailAddressConfirmed)
            {
                yield return new CommandResponse
                {
                    Message = "You must have a confirmed email address to use this command."
                };

                yield break;
            }

            var stalks = this.channelConfiguration[this.CommandSource].Stalks.Values;

            var disabled = stalks.Where(x => !x.IsEnabled);
            var expired = stalks.Where(x => x.ExpiryTime != null && x.ExpiryTime < DateTime.Now);
            var active = stalks.Where(x => x.IsActive());

            var body = string.Format(
                this.templates.EmailStalkReport,
                this.recentChangeHandler.FormatStalkListForEmail(active, botUser),
                this.recentChangeHandler.FormatStalkListForEmail(disabled, botUser),
                this.recentChangeHandler.FormatStalkListForEmail(expired, botUser),
                this.CommandSource
            );

            this.emailHelper.SendEmail(
                body,
                string.Format(this.templates.EmailStalkReportSubject, this.CommandSource),
                null,
                botUser);

            yield return new CommandResponse
            {
                Message = "Stalk report sent via email"
            };
        }

        [SubcommandInvocation("or")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help("<Identifier> <user|page|summary|xml> <Match...>",
            "Sets the stalk configuration of the specified stalk to the logical OR of the current configuration, and a specified user, page, or edit summary; or XML tree (advanced).")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> OrMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newNode = this.CreateNode(newStalkType, newTarget);
            if (stalk.SearchTree.GetType() == typeof(OrNode))
            {
                ((OrNode) stalk.SearchTree).ChildNodes.Add(newNode);
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

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("and")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help("<Identifier> <user|page|summary|xml> <Match...>",
            "Sets the stalk configuration of the specified stalk to the logical AND of the current configuration, and a specified user, page, or edit summary; or XML tree (advanced).")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> AndMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }


            var newStalkType = tokenList.PopFromFront();

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newNode = this.CreateNode(newStalkType, newTarget);

            if (stalk.SearchTree.GetType() == typeof(AndNode))
            {
                ((AndNode) stalk.SearchTree).ChildNodes.Add(newNode);
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

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("enabled")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help("<Identifier> <true|false>", "Marks a stalk as enabled or disabled")]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        protected IEnumerable<CommandResponse> EnabledMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
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

            this.channelConfiguration[this.CommandSource].Stalks[stalkName].IsEnabled = enabled;

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("enable")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(1)]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> EnableAlias()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            
            this.Arguments.Clear();
            this.Arguments.Add(stalkName);
            this.Arguments.Add("true");

            return this.EnabledMode();
        }

        [SubcommandInvocation("disable")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(1)]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DisableAlias()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            
            this.Arguments.Clear();
            this.Arguments.Add(stalkName);
            this.Arguments.Add("false");

            return this.EnabledMode();
        }

        [SubcommandInvocation("expiry")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help("<Identifier> <Expiry>", "Sets the expiry date/time of the specified stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ExpiryMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var date = string.Join(" ", tokenList);

            var expiryTime = DateTime.Parse(date);
            this.channelConfiguration[this.CommandSource].Stalks[stalkName].ExpiryTime = expiryTime;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set expiry attribute on stalk {0} to {1}",
                    stalkName,
                    expiryTime.ToString(this.config.DateFormat))
            };

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("description"), SubcommandInvocation("desc")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(1)]
        [Help("<Identifier> <Description...>", "Sets the description of the specified stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DescriptionMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var descr = string.Join(" ", tokenList);

            if (string.IsNullOrWhiteSpace(descr))
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].Description = null;

                yield return new CommandResponse
                {
                    Message = string.Format("Cleared description attribute on stalk {0}", stalkName)
                };
            }
            else
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].Description = descr;

                yield return new CommandResponse
                {
                    Message = string.Format("Set description attribute on stalk {0} to {1}", stalkName, descr)
                };
            }

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("list")]
        [CommandFlag(CLFlag.Standard)]
        [Help("", "Lists all active stalks")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ListMode()
        {
            var activeStalks = this.channelConfiguration[this.CommandSource]
                .Stalks.Values.Where(x => x.IsActive())
                .ToList();

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
                    "Identifier: {0}, Last modified: {1}, Type: Complex {2}",
                    stalk.Identifier,
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

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("set")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help(
            "<Identifier> <user|page|summary|xml> <Match...>",
            "Sets the stalk configuration of the specified stalk to specified user, page, or edit summary. Alternatively, manually specify an XML tree (advanced).")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> SetMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newroot = this.CreateNode(newStalkType, newTarget);
            stalk.SearchTree = newroot;

            yield return new CommandResponse
            {
                Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", newStalkType, stalkName, newroot)
            };

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("delete"), SubcommandInvocation("del")]
        [RequiredArguments(1)]
        [CommandFlag(AccessFlags.Configuration)]
        [Help("<identifier>", "Deletes a stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DeleteMode()
        {
            var stalkName = this.Arguments.First();

            this.channelConfiguration[this.CommandSource].Stalks.Remove(stalkName);

            yield return new CommandResponse
            {
                Message = string.Format("Deleted stalk {0}", stalkName)
            };

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("export")]
        [CommandFlag(CLFlag.Standard)]
        [RequiredArguments(1)]
        [Help("<identifier>", "Retrieves the XML search tree of a specific stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ExportMode()
        {
            var stalkName = this.Arguments.First();

            var searchTree = this.channelConfiguration[this.CommandSource].Stalks[stalkName].SearchTree;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "{0}: {1}",
                    stalkName,
                    this.stalkNodeFactory.ToXml(new XmlDocument(), searchTree).OuterXml
                )
            };
        }

        [RequiredArguments(1)]
        [SubcommandInvocation("add")]
        [CommandFlag(AccessFlags.Configuration)]
        [Help("<identifier>", "Adds a new unconfigured stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> AddMode()
        {
            var tokenList = this.Arguments;

            var stalkName = tokenList.First();
            var stalk = new ComplexStalk(stalkName) {Channel = this.CommandSource};

            this.channelConfiguration[this.CommandSource].Stalks.Add(stalk.Identifier, stalk);

            yield return new CommandResponse
            {
                Message = string.Format("Added disabled stalk {0} with CSL value: {1}", stalkName, stalk.SearchTree)
            };

            this.channelConfiguration.Save();
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