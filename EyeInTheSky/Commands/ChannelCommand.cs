namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("channel")]
    public class ChannelCommand : CommandBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IAppConfiguration appConfig;

        public ChannelCommand(
            string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IChannelConfiguration channelConfiguration,
            IBotUserConfiguration botUserConfiguration,
            IAppConfiguration appConfig)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
            this.channelConfiguration = channelConfiguration;
            this.botUserConfiguration = botUserConfiguration;
            this.appConfig = appConfig;
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
                case "subscribe":
                case "unsubscribe":
                case "list":
                    return this.FlagService.UserHasFlag(this.User, CLFlag.Standard, this.CommandSource);
            }

            if (tokenList.Count < 1)
            {
                return base.CanExecute();
            }

            var channel = tokenList.PopFromFront();

            switch (mode)
            {
                case "join":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null);
                case "part":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null) ||
                           this.FlagService.UserHasFlag(this.User, AccessFlags.LocalAdmin, channel);
                default:
                    return false;
            }
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(1, this.Arguments.Count());
            }

            var mode = tokenList.PopFromFront();

            switch (mode)
            {
                case "subscribe":
                    return this.SubscribeMode();
                case "unsubscribe":
                    return this.UnsubscribeMode();
                case "list":
                    return this.ListMode();
            }

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count());
            }

            var channel = tokenList.PopFromFront();

            switch (mode)
            {
                case "join":
                    return this.JoinMode(channel);
                case "part":
                    return this.PartMode(channel);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> ListMode()
        {
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

            bool responded = false;
            
            foreach (var channel in this.channelConfiguration.Items)
            {
                // check for channel subscription
                var items = new List<string>();
                if (channel.Users.Any(x => x.Mask.ToString() == botUser.Identifier && x.Subscribed))
                {
                    items.Add("subscribed to channel");
                }

                var stalkKeys = string.Join(
                    ", ",
                    channel.Stalks
                        .Where(
                            x => x.Value.Subscribers
                                .Any(s => s.Mask.ToString() == botUser.Identifier))
                        .Select(x => x.Key));
                
                // check for individual stalk subscriptions
                if (!string.IsNullOrWhiteSpace(stalkKeys))
                {
                    items.Add("subscribed to stalks (" + stalkKeys + ")");
                }

                if (items.Any())
                {
                    yield return new CommandResponse
                    {
                        Message = string.Format("{0}: {1}", channel.Identifier, string.Join("; ", items)),
                        Destination = CommandResponseDestination.PrivateMessage
                    };
                    responded = true;
                }
            }

            if (!responded)
            {
                yield return new CommandResponse
                {
                    Message = "No subscriptions found",
                    Destination = CommandResponseDestination.PrivateMessage
                };
            }
        }

        private IEnumerable<CommandResponse> UnsubscribeMode()
        {
            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
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

            var channelUser = this.channelConfiguration[this.CommandSource]
                .Users.FirstOrDefault(x => x.Mask.ToString() == botUser.Identifier);

            if (channelUser == null)
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are not subscribed to notifications for {0}", this.CommandSource)
                };

                yield break;
            }

            channelUser.Subscribed = false;
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Unsubscribed from all notifications for {0}", this.CommandSource)
            };
        }

        private IEnumerable<CommandResponse> SubscribeMode()
        {
            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
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

            var channelUser = this.channelConfiguration[this.CommandSource]
                .Users.FirstOrDefault(x => x.Mask.ToString() == botUser.Identifier);
            if (channelUser == null)
            {
                channelUser = new ChannelUser(botUser.Mask);
                this.channelConfiguration[this.CommandSource].Users.Add(channelUser);
            }

            if (channelUser.Subscribed)
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are already subscribed to notifications for {0}", this.CommandSource)
                };

                yield break;
            }

            channelUser.Subscribed = true;
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Subscribed to all notifications for {0}", this.CommandSource)
            };
        }

        private IEnumerable<CommandResponse> PartMode(string channel)
        {
            if (this.appConfig.FreenodeChannel == channel)
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Cannot leave default channel"
                    }
                };
            }

            if (this.channelConfiguration.ContainsKey(channel))
            {
                this.channelConfiguration.Remove(channel);
                this.channelConfiguration.Save();
            }

            if (this.Client.Channels.ContainsKey(channel))
            {
                this.Client.PartChannel(channel, string.Format("requested by {0}", this.User.Nickname));
            }

            return null;
        }

        private IEnumerable<CommandResponse> JoinMode(string channel)
        {
            this.channelConfiguration.Add(new IrcChannel(channel));
            this.channelConfiguration.Save();
            this.Client.JoinChannel(channel);
            this.Client.SendMessage(
                channel,
                string.Format("My presence in {0} was requested by {1}.", channel, this.User));

            yield break;
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            var help = new Dictionary<string, HelpMessage>
            {
                {
                    "subscribe",
                    new HelpMessage(
                        this.CommandName,
                        "subscribe",
                        "Subscribe to email notifications from all stalks in this channel")
                },
                {
                    "unsubscribe",
                    new HelpMessage(
                        this.CommandName,
                        "unsubscribe",
                        "Unsubscribe from email notifications from all stalks in this channel")
                },
                {
                    "list",
                    new HelpMessage(
                        this.CommandName,
                        "list",
                        "Retrieve a list of channels in which you have active subscriptions")
                }
            };

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null))
            {
                help.Add(
                    "join",
                    new HelpMessage(
                        this.CommandName,
                        "join <channel>",
                        "Requests the bot joins a channel"));

                help.Add(
                    "part",
                    new HelpMessage(
                        this.CommandName,
                        "part <channel>",
                        "Requests the bot leaves a channel, removing all configuration for that channel."));
            }

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.LocalAdmin, this.CommandSource))
            {
                if (!help.ContainsKey("part"))
                {
                    help.Add(
                        "part",
                        new HelpMessage(
                            this.CommandName,
                            "part <channel>",
                            "Requests the bot leaves a channel, removing all configuration for that channel."));
                }
            }

            return help;
        }
    }
}