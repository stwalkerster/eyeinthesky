namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
    using EyeInTheSky.Services.Email.Interfaces;
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
    [CommandFlag(AccessFlags.User)]
    public class StalkCommand : CommandBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IAppConfiguration config;
        private readonly INotificationTemplates templates;
        private readonly IEmailHelper emailHelper;
        private readonly IXmlCacheService xmlCacheService;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly ISubscriptionHelper subscriptionHelper;
        private readonly IIrcClient wikimediaClient;
        private readonly IEmailTemplateFormatter emailTemplateFormatter;
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
            IXmlCacheService xmlCacheService,
            IBotUserConfiguration botUserConfiguration,
            ISubscriptionHelper subscriptionHelper,
            IIrcClient wikimediaClient,
            IEmailTemplateFormatter emailTemplateFormatter
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
            this.xmlCacheService = xmlCacheService;
            this.botUserConfiguration = botUserConfiguration;
            this.subscriptionHelper = subscriptionHelper;
            this.wikimediaClient = wikimediaClient;
            this.emailTemplateFormatter = emailTemplateFormatter;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        protected override IEnumerable<CommandResponse> OnPreRun(out bool abort)
        {
            abort = false;

            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
            }

            return null;
        }

        [SubcommandInvocation("unsubscribe")]
        [RequiredArguments(1)]
        [Help("<Identifier>", "Unsubscribes from email notifications for this stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> UnsubscribeMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            var ircChannel = this.channelConfiguration[this.CommandSource];
            if (!ircChannel.Stalks.ContainsKey(stalkName))
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

            var stalk = ircChannel.Stalks[stalkName];
            SubscriptionSource subscriptionSource;
            var result = this.subscriptionHelper.UnsubscribeStalk(botUser.Mask, ircChannel, stalk, out subscriptionSource);
            this.channelConfiguration.Save();

            if (result)
            {
                yield return new CommandResponse
                {
                    Message = string.Format("Unsubscribed from notifications for stalk {0}", stalkName)
                };
            }
            else
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are not subscribed to notifications for stalk {0}", stalkName)
                };
            }
        }

        [SubcommandInvocation("subscribe")]
        [RequiredArguments(1)]
        [Help("<Identifier>", "Subscribes to email notifications for this stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> SubscribeMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            var ircChannel = this.channelConfiguration[this.CommandSource];
            if (!ircChannel.Stalks.ContainsKey(stalkName))
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

            var stalk = ircChannel.Stalks[stalkName];

            SubscriptionSource subscriptionSource;
            var result = this.subscriptionHelper.SubscribeStalk(botUser.Mask, ircChannel, stalk, out subscriptionSource);
            this.channelConfiguration.Save();

            if (result)
            {
                yield return new CommandResponse
                {
                    Message = string.Format("Subscribed to notifications for stalk {0}", stalkName)
                };
            }
            else
            {
                var message = string.Format("You are already subscribed to notifications for stalk {0}", stalkName);
                if (subscriptionSource == SubscriptionSource.Channel)
                {
                    message = string.Format(
                        "You are already subscribed to notifications for {1}, including stalk {0}", stalkName,
                        ircChannel.Identifier);

                }

                yield return new CommandResponse
                {
                    Message = message
                };
            }
        }

        [SubcommandInvocation("report")]
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
            var expired = stalks.Where(x => x.ExpiryTime != null && x.ExpiryTime < DateTime.UtcNow);
            var active = stalks.Where(x => x.IsActive());

            var body = string.Format(
                this.templates.EmailStalkReport,
                this.emailTemplateFormatter.FormatStalkListForEmail(active, botUser),
                this.emailTemplateFormatter.FormatStalkListForEmail(disabled, botUser),
                this.emailTemplateFormatter.FormatStalkListForEmail(expired, botUser),
                this.CommandSource
            );

            this.emailHelper.SendEmail(
                body,
                string.Format(this.templates.EmailStalkReportSubject, this.CommandSource),
                null,
                botUser,
                null);

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
        [Help(new[]{"<Identifier> <Expiry Date>","<Identifier> <Duration>", "<Identifier> never"},
            "Sets the expiry date/time of the specified stalk")]
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

            DateTime expiryTime;
            TimeSpan expiryDuration;
            if (date == "never" || date == "infinite" || date == "infinity")
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].ExpiryTime = null;
                yield return new CommandResponse
                {
                    Message = string.Format("Removed expiry from stalk {0}", stalkName)
                };

            }
            else if (DateTime.TryParse(date, out expiryTime))
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].ExpiryTime = expiryTime;
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "Set expiry attribute on stalk {0} to {1}",
                        stalkName,
                        expiryTime.ToString(this.config.DateFormat))
                };
            }
            else if (TimeSpan.TryParse(date, out expiryDuration))
            {
                expiryTime = DateTime.UtcNow + expiryDuration;
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].ExpiryTime = expiryTime;
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "Set expiry attribute on stalk {0} to {1}",
                        stalkName,
                        expiryTime.ToString(this.config.DateFormat))
                };
            }
            else
            {
                throw new CommandErrorException(string.Format(
                    "Unable to parse date from '{0}'. If you mean to remove the expiry date, please specify \"never\".",
                    date));
            }

            this.channelConfiguration.Save();
        }

        [SubcommandInvocation("dynamicexpiry")]
        [CommandFlag(AccessFlags.Configuration)]
        [RequiredArguments(2)]
        [Help(new[]{"<Identifier> <Interval>", "<Identifier> never"},
            "Sets the dynamic expiry duration of the specified stalk. ")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DynamicExpiryMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var date = string.Join(" ", tokenList);

            TimeSpan expiryDuration;
            if (date == "never" || date == "infinite" || date == "infinity")
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].DynamicExpiry = null;
                yield return new CommandResponse
                {
                    Message = string.Format("Removed dynamic expiry duration from stalk {0}", stalkName)
                };

            }
            else if (TimeSpan.TryParse(date, out expiryDuration))
            {
                this.channelConfiguration[this.CommandSource].Stalks[stalkName].DynamicExpiry = expiryDuration;
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "Set dynamic expiry attribute on stalk {0} to {1}",
                        stalkName,
                        expiryDuration.ToString(this.config.TimeSpanFormat))
                };
            }
            else
            {
                throw new CommandErrorException(string.Format(
                    "Unable to parse time span from '{0}'. If you mean to remove the dynamic expiry duration, please specify \"never\".",
                    date));
            }

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

            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            this.channelConfiguration[this.CommandSource].Stalks.Remove(stalkName);
            ChannelConfiguration.IndividualMatchDuration.RemoveLabelled(this.CommandSource, stalkName);

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

            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

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
        [SubcommandInvocation("add"), SubcommandInvocation("create")]
        [CommandFlag(AccessFlags.Configuration)]
        [Help("<identifier>", "Adds a new unconfigured stalk")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> AddMode()
        {
            var tokenList = this.Arguments;

            var stalkName = tokenList.First();
            var stalk = new ComplexStalk(stalkName)
                {Channel = this.CommandSource, WatchChannel = this.config.WikimediaChannel};

            this.channelConfiguration[this.CommandSource].Stalks.Add(stalk.Identifier, stalk);

            yield return new CommandResponse
            {
                Message = string.Format("Added disabled stalk {0} with CSL value: {1}", stalkName, stalk.SearchTree)
            };

            this.channelConfiguration.Save();
        }

        [RequiredArguments(3)]
        [SubcommandInvocation("clone")]
        [CommandFlag(AccessFlags.Configuration)]
        [Help("<source channel> <source identifier> <target identifier>", "Clones an existing stalk from a different channel")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> CloneMode()
        {
            var tokenList = this.Arguments;
            var sourceChannel = tokenList[0];
            var sourceIdentifier = tokenList[1];
            var targetIdentifier = tokenList[2];

            var sourceChannelConfig = this.channelConfiguration.ContainsKey(sourceChannel)
                ? this.channelConfiguration[sourceChannel]
                : null;

            if (sourceChannelConfig == null)
            {
                throw new CommandErrorException(string.Format("The channel {0} does not exist in configuration", sourceChannel));
            }

            var sourceStalk = sourceChannelConfig.Stalks.ContainsKey(sourceIdentifier)
                ? sourceChannelConfig.Stalks[sourceIdentifier]
                : null;

            if (sourceStalk == null)
            {
                throw new CommandErrorException(string.Format("The channel {0} does not exist in configuration", sourceChannel));
            }

            if (this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(targetIdentifier))
            {
                throw new CommandErrorException(string.Format("The identifier {0} already exists in this channel!",
                    targetIdentifier));
            }

            var tree = sourceStalk.SearchTree;

            // bounce it via XML to ensure its a) different objects and b) saves properly
            tree = this.stalkNodeFactory.NewFromXmlFragment(this.stalkNodeFactory.ToXml(new XmlDocument(), tree));

            var newStalk = new ComplexStalk(targetIdentifier)
            {
                Channel = this.CommandSource,
                Description = string.Format("{0}; based on stalk {1} from {2}", sourceStalk.Description, sourceIdentifier, sourceChannel),
                ExpiryTime = sourceStalk.ExpiryTime,
                IsEnabled = sourceStalk.IsEnabled,
                SearchTree = tree,
                WatchChannel = sourceStalk.WatchChannel,
                DynamicExpiry = sourceStalk.DynamicExpiry,
                LastTriggerTime = sourceStalk.LastTriggerTime
            };

            this.channelConfiguration[this.CommandSource].Stalks.Add(newStalk.Identifier, newStalk);
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Created new {1} stalk {0}{2} with CSL {3}",
                    newStalk.Identifier,
                    newStalk.IsEnabled ? "enabled" : "disabled",
                    newStalk.ExpiryTime.HasValue
                        ? newStalk.ExpiryTime < DateTime.UtcNow
                            ? string.Format(" (expired {0:%d}d {0:%h}h {0:%m}m ago)", newStalk.ExpiryTime.Value - DateTime.UtcNow)
                            : string.Format(" (expiring in {0:%d}d {0:%h}h {0:%m}m)", newStalk.ExpiryTime.Value - DateTime.UtcNow)
                        : string.Empty,
                    tree
                )
            };
        }

        [RequiredArguments(2)]
        [SubcommandInvocation("watchchannel")]
        [CommandFlag(AccessFlags.Configuration)]
        [Help("<identifier> <channel>", "Changes the monitored feed to a different wiki")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> WatchChannelMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var stalkName = tokenList.PopFromFront();
            if (!this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }

            var stalk = this.channelConfiguration[this.CommandSource].Stalks[stalkName];

            var oldWatchChannel = stalk.WatchChannel;

            stalk.WatchChannel = tokenList.PopFromFront();

            if (!this.wikimediaClient.Channels.ContainsKey(stalk.WatchChannel))
            {
                this.wikimediaClient.JoinChannel(stalk.WatchChannel);
            }

            var remainingStalksOnChannel = this.channelConfiguration.Items.Select(x => x.Stalks)
                .Aggregate(
                    new List<IStalk>(),
                    (agg, cur) =>
                    {
                        agg.AddRange(cur.Values.Where(x => x.WatchChannel == oldWatchChannel));
                        return agg;
                    });

            if (remainingStalksOnChannel.Count == 0)
            {
                this.wikimediaClient.PartChannel(oldWatchChannel, "Nothing left to watch here");
            }

            Debugger.Break();

            yield return new CommandResponse
            {
                Message = string.Format("Set watch channel for stalk {0} to {1}", stalkName, stalk.WatchChannel)
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