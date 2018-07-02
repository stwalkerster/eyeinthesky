namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
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
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

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
            var accountName = tokenList.PopFromFront();
            var userMask = "$a:" + accountName;
            var flagChanges = tokenList.PopFromFront();

            if (!this.botUserConfiguration.ContainsKey(userMask))
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "No such user is currently registered"
                    }
                };
            }

            switch (mode)
            {
                case "global":
                    return this.GlobalMode(userMask, flagChanges, accountName);
                case "local":
                    return this.LocalMode(userMask, flagChanges, accountName);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> LocalMode(string userMask, string flagChanges, string accountName)
        {
            var user = this.channelConfiguration[this.CommandSource]
                .Users.FirstOrDefault(x => x.Mask.ToString() == userMask);
            if (user == null)
            {
                user = new ChannelUser(new IrcUserMask(userMask, this.Client));
                this.channelConfiguration[this.CommandSource].Users.Add(user);
            }

            var currentFlags = new HashSet<string>();
            if (user.LocalFlags != null)
            {
                currentFlags = user.LocalFlags.ToCharArray()
                    .Aggregate(
                        new HashSet<string>(),
                        (s, i) =>
                        {
                            s.Add(i.ToString());
                            return s;
                        });
            }

            // GlobalAdmin and Owner should never be granted locally - too much power.
            // Standard is granted by default, so no sense to remove it.
            var preventedChanges = new[] {AccessFlags.GlobalAdmin, CLFlag.Owner, CLFlag.Standard};
            
            var updatedFlags = this.ApplyFlagChanges(ref flagChanges, currentFlags, preventedChanges);

            var newFlagString = string.Join(string.Empty, updatedFlags.OrderBy(x => x));

            user.LocalFlags = newFlagString;
            this.channelConfiguration.Save();

            if (string.IsNullOrEmpty(flagChanges))
            {
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "No valid changes to local flags in {1} for {0} proposed.",
                        accountName,
                        this.CommandSource)
                };
            }
            else
            {
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "Updated local flags in {3} for {0}, using these changes: {1}. New locally-held flags are {2}",
                        accountName,
                        flagChanges,
                        newFlagString,
                        this.CommandSource)
                };
            }
        }

        private IEnumerable<CommandResponse> GlobalMode(string userMask, string flagChanges, string accountName)
        {
            var currentFlags = new HashSet<string>();

            if (this.botUserConfiguration[userMask].GlobalFlags != null)
            {
                currentFlags = this.botUserConfiguration[userMask]
                    .GlobalFlags.ToCharArray()
                    .Aggregate(
                        currentFlags,
                        (s, i) =>
                        {
                            s.Add(i.ToString());
                            return s;
                        });
            }

            // Standard is granted by default, so no sense to remove it.
            var preventedChanges = new List<string> {CLFlag.Standard};

            if (!this.FlagService.UserHasFlag(this.User, CLFlag.Owner, null))
            {
                preventedChanges.Add(CLFlag.Owner);
            }
            
            var updatedFlags = this.ApplyFlagChanges(ref flagChanges, currentFlags, preventedChanges);

            this.botUserConfiguration[userMask].GlobalFlags = string.Join(string.Empty, updatedFlags);
            this.botUserConfiguration.Save();

            updatedFlags.Add("S");

            if (string.IsNullOrEmpty(flagChanges))
            {
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "No valid changes to global flags for {0} proposed.",
                        accountName)
                };
            }
            else
            {
                yield return new CommandResponse
                {
                    Message = string.Format(
                        "Updated global flags for {0}, using these changes: {1}. New globally-held flags are {2}",
                        accountName,
                        flagChanges,
                        string.Join(string.Empty, updatedFlags.OrderBy(x => x)))
                };
            }
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
                        "flags <global|local> <account> <changes>",
                        "Modifies the flags granted to the usermask in either a global scope or in the local channel"));
            }
            else if (this.FlagService.UserHasFlag(this.User, AccessFlags.ChannelAdmin, this.CommandSource))
            {
                help.Add(
                    "flags",
                    new HelpMessage(
                        this.CommandName,
                        "flags local <usermask> <account`1>",
                        "Modifies the flags granted to the usermask in the local channel"));
            }

            return help;
        }

        private HashSet<string> ApplyFlagChanges(ref string changes, IEnumerable<string> original, IList<string> preventedChanges)
        {
            var result = new HashSet<string>(original);
            bool addMode = true;

            string newChanges = "";
            
            foreach (var changeChar in changes)
            {
                var c = changeChar.ToString();
                
                if (c == "-")
                {
                    addMode = false;
                    newChanges += c;
                    continue;
                }

                if (c == "+")
                {
                    addMode = true;
                    newChanges += c;
                    continue;
                }

                if (c == "*" && !addMode)
                {
                    foreach (var i in new List<string>(result))
                    {
                        if (preventedChanges.Contains(c))
                        {
                            continue;
                        }
                        
                        result.Remove(i);
                        newChanges += i;    
                    }
                    
                    continue;
                }

                if (addMode)
                {
                    if (!AccessFlags.ValidFlags.Contains(c))
                    {
                        continue;
                    }

                    if (preventedChanges.Contains(c))
                    {
                        continue;
                    }

                    if (result.Contains(c))
                    {
                        // no-op
                        continue;
                    }
                    
                    newChanges += c;
                    result.Add(c);
                }
                else
                {
                    if (preventedChanges.Contains(c))
                    {
                        continue;
                    }

                    if (!result.Contains(c))
                    {
                        // no-op
                        continue;
                    }
                    
                    newChanges += c;
                    result.Remove(c);
                }
            }

            newChanges = newChanges.TrimEnd('+', '-');
            
            changes = newChanges;
            return result;
        }
    }
}