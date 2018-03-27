namespace EyeInTheSky.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.StalkNodes;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("stalk")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class StalkCommand : StalkCommandBase
    {
        private readonly IAppConfiguration config;

        public StalkCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            StalkConfiguration stalkConfig,
            IStalkNodeFactory stalkNodeFactory,
            IAppConfiguration config) : base(
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
            this.config = config;
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
                case "list":
                    return this.ListMode();
            }

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count(), mode);
            }
            
            var stalkName = tokenList.PopFromFront();
            if (!this.StalkConfig.ContainsKey(stalkName))
            {
                throw new CommandErrorException(string.Format("Can't find the stalk '{0}'!", stalkName));
            }
            
            switch (mode)
            {
                case "del":
                    return this.DeleteMode(stalkName);
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

        private IEnumerable<CommandResponse> OrMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "or");
            }

            var newStalkType = tokenList.PopFromFront();
            
            var stalk = this.StalkConfig[stalkName];
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
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> AndMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "and");
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.StalkConfig[stalkName];
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

            this.StalkConfig.Save();
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
            
            this.StalkConfig[stalkName].IsEnabled = enabled;
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ExpiryMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "expiry");
            }

            var date = string.Join(" ", tokenList);

            var expiryTime = DateTime.Parse(date);
            this.StalkConfig[stalkName].ExpiryTime = expiryTime;

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Set expiry attribute on stalk {0} to {1}",
                    stalkName,
                    expiryTime.ToString(this.config.DateFormat))
            };
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DescriptionMode(List<string> tokenList, string stalkName)
        {
            var descr = string.Join(" ", tokenList);

            if (string.IsNullOrWhiteSpace(descr))
            {
                this.StalkConfig[stalkName].Description = null;
                
                yield return new CommandResponse
                {
                    Message = string.Format("Cleared description attribute on stalk {0}", stalkName)
                };
            }
            else
            {
                this.StalkConfig[stalkName].Description = descr;

                yield return new CommandResponse
                {
                    Message = string.Format("Set description attribute on stalk {0} to {1}", stalkName, descr)
                };
            }

            this.StalkConfig.Save();
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
            
            this.StalkConfig[stalkName].MailEnabled = mail;
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> ListMode()
        {
            var activeStalks = this.StalkConfig.StalkList.Where(x => x.IsActive()).ToList();

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

            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> SetMode(List<string> tokenList, string stalkName)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count(), "set");
            }

            var newStalkType = tokenList.PopFromFront();

            var stalk = this.StalkConfig[stalkName];
            var newTarget = string.Join(" ", tokenList);

            var newroot = this.CreateNode(newStalkType, newTarget);
            stalk.SearchTree = newroot;

            yield return new CommandResponse
            {
                Message = string.Format("Set {0} for stalk {1} with CSL value: {2}", newStalkType, stalkName, newroot)
            };
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> DeleteMode(string stalkName)
        {
            this.StalkConfig.Remove(stalkName);
            
            yield return new CommandResponse
            {
                Message = string.Format("Deleted stalk {0}", stalkName)
            };
            
            this.StalkConfig.Save();
        }

        private IEnumerable<CommandResponse> AddMode(List<string> tokenList)
        {
            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count(), "add");
            }

            var stalkName = tokenList.First();
            var stalk = new ComplexStalk(stalkName);

            this.StalkConfig.Add(stalkName, stalk);
            
            yield return new CommandResponse
            {
                Message = string.Format("Added disabled stalk {0} with CSL value: {1}", stalkName, stalk.SearchTree)
            };
            
            this.StalkConfig.Save();
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
    }
}
