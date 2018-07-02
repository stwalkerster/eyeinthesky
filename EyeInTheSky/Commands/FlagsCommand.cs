namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Policy;

    using Castle.Core.Logging;

    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Services.Interfaces;

    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("flags")]
    public class FlagsCommand : CommandBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IChannelConfiguration channelConfiguration;

        public FlagsCommand(
            string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IBotUserConfiguration botUserConfiguration,
            IChannelConfiguration channelConfiguration)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
            this.botUserConfiguration = botUserConfiguration;
            this.channelConfiguration = channelConfiguration;
        }

        public override bool CanExecute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                return base.CanExecute();
            }

            var mode = tokenList.PopFromFront();

            switch (mode)
            {
                case "global":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null);
                case "local":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null) ||
                           this.FlagService.UserHasFlag(this.User, AccessFlags.ChannelAdmin, this.CommandSource);
                default:
                    return false;
            }
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(3, this.Arguments.Count());
            }

            var mode = tokenList.PopFromFront();
            var userMask = tokenList.PopFromFront();
            var flagChanges = tokenList.PopFromFront();

            if (!this.botUserConfiguration.ContainsKey(userMask))
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "No such user is currently registered",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }
            
            switch (mode)
            {
                case "global":
                    return this.GlobalMode(userMask, flagChanges);
                case "local":
                    return this.LocalMode(userMask, flagChanges);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> LocalMode(string userMask, string flagChanges)
        {
            var user = this.channelConfiguration[this.CommandSource].Users.FirstOrDefault(x => x.Mask.ToString() == userMask);
            if (user == null)
            {
                user = new ChannelUser(new IrcUserMask(userMask, this.Client));
                this.channelConfiguration[this.CommandSource].Users.Add(user);
            }
            
            var currentFlags = user.LocalFlags.ToCharArray()
                .Aggregate(new HashSet<string>(), (s, i) =>
                {
                    s.Add(i.ToString());
                    return s;
                });

            var updatedFlags = this.ApplyFlagChanges(flagChanges, currentFlags);

            var newFlagString = string.Join(string.Empty, updatedFlags);
            
            user.LocalFlags = newFlagString;
            this.channelConfiguration.Save();
            
            yield return new CommandResponse
            {
                Message = string.Format(
                    "Updated local flags for {0}, using these changes: {1}. New locally-held flags are {2}",
                    userMask, flagChanges, newFlagString)
            };
        }

        private IEnumerable<CommandResponse> GlobalMode(string userMask, string flagChanges)
        {
            var currentFlags = this.botUserConfiguration[userMask].GlobalFlags.ToCharArray()
                .Aggregate(new HashSet<string>(), (s, i) =>
                {
                    s.Add(i.ToString());
                    return s;
                });

            var updatedFlags = this.ApplyFlagChanges(flagChanges, currentFlags);

            var newFlagString = string.Join(string.Empty, updatedFlags);
            
            this.botUserConfiguration[userMask].GlobalFlags = newFlagString;
            this.botUserConfiguration.Save();
            
            yield return new CommandResponse
            {
                Message = string.Format(
                    "Updated global flags for {0}, using these changes: {1}. New globally-held flags are {2}",
                    userMask, flagChanges, newFlagString)
            };
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            var help = new Dictionary<string, HelpMessage>();

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null))
            {
                help.Add(
                    "flags",
                    new HelpMessage(
                        this.CommandName,
                        "flags <global|local> <usermask> <changes>",
                        "Modifies the flags granted to the usermask in either a global scope or in the local channel"));
            }
            else if (this.FlagService.UserHasFlag(this.User, AccessFlags.ChannelAdmin, this.CommandSource))
            {
                help.Add(
                    "flags",
                    new HelpMessage(
                        this.CommandName,
                        "flags local <usermask> <changes>",
                        "Modifies the flags granted to the usermask in the local channel"));
            }

            return help;
        }
        
        private IEnumerable<string> ApplyFlagChanges(string changes, IEnumerable<string> original)
        {
            var result = new HashSet<string>(original);
            bool addMode = true;
            
            foreach (var c in changes)
            {
                if (c == '-')
                {
                    addMode = false;
                    continue;
                }

                if (c == '+')
                {
                    addMode = true;
                    continue;
                }

                if (c == '*' && !addMode)
                {
                    result.Clear();
                    continue;
                }

                if (!AccessFlags.ValidFlags.Contains(c.ToString()))
                {
                    continue;
                }

                if (addMode)
                {
                    result.Add(c.ToString());
                }
                else
                {
                    result.Remove(c.ToString());
                }
            }

            return result;
        }
    }
}