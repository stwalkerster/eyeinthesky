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
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("flags")]
    [CommandFlag(AccessFlags.GlobalAdmin, true)]
    [CommandFlag(AccessFlags.LocalAdmin)]
    public class FlagsCommand : CommandBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IChannelConfiguration channelConfiguration;

        public FlagsCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
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

        [SubcommandInvocation("local")]
        [CommandFlag(AccessFlags.GlobalAdmin, true)]
        [CommandFlag(AccessFlags.LocalAdmin)]
        [RequiredArguments(2)]
        [Help("<account> <changes>", "Modifies the flags granted to the usermask in the local channel")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> LocalMode()
        {
            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
            }

            var tokenList = (List<string>) this.Arguments;
            var accountName = tokenList.PopFromFront();
            var userMask = "$a:" + accountName;
            var flagChanges = tokenList.PopFromFront();

            if (!this.botUserConfiguration.ContainsKey(userMask))
            {
                yield return new CommandResponse
                {
                    Message = "No such user is currently registered"
                };
                yield break;
            }

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
            // User is granted based on login to NickServ account, also makes no sense to remove it
            var preventedChanges = new[] {AccessFlags.GlobalAdmin, CLFlag.Owner, CLFlag.Standard, AccessFlags.User};

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
                        string.IsNullOrWhiteSpace(newFlagString) ? "(none)" : newFlagString,
                        this.CommandSource)
                };
            }
        }

        [SubcommandInvocation("global")]
        [CommandFlag(AccessFlags.GlobalAdmin, true)]
        [RequiredArguments(2)]
        [Help("<account> <changes>", "Modifies the flags granted to the usermask in the global scope")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> GlobalMode()
        {
            var tokenList = (List<string>) this.Arguments;
            var accountName = tokenList.PopFromFront();
            var userMask = "$a:" + accountName;
            var flagChanges = tokenList.PopFromFront();

            if (!this.botUserConfiguration.ContainsKey(userMask))
            {
                yield return new CommandResponse
                {
                    Message = "No such user is currently registered"
                };
                yield break;
            }

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
            // User is granted based on login to NickServ account, also makes no sense to remove it
            var preventedChanges = new List<string> {CLFlag.Standard, AccessFlags.User};

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

        private HashSet<string> ApplyFlagChanges(
            ref string changes,
            IEnumerable<string> original,
            IList<string> preventedChanges)
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