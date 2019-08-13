namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("channel")]
    [CommandFlag(AccessFlags.User)]
    public class ChannelCommand : CommandBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IAppConfiguration appConfig;
        private readonly ISubscriptionHelper subscriptionHelper;

        public ChannelCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IChannelConfiguration channelConfiguration,
            IBotUserConfiguration botUserConfiguration,
            IAppConfiguration appConfig, ISubscriptionHelper subscriptionHelper)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
            this.channelConfiguration = channelConfiguration;
            this.botUserConfiguration = botUserConfiguration;
            this.appConfig = appConfig;
            this.subscriptionHelper = subscriptionHelper;
        }

        [SubcommandInvocation("list")]
        [Help("", "Retrieve a list of channels in which you have active subscriptions")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ListMode()
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

        [SubcommandInvocation("unsubscribe")]
        [Help("", "Unsubscribe from email notifications from all stalks in this channel")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> UnsubscribeMode()
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

            if (!this.subscriptionHelper.UnsubscribeChannel(botUser.Mask, this.channelConfiguration[this.CommandSource]))
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are not subscribed to notifications for {0}", this.CommandSource)
                };

                yield break;
            }

            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Unsubscribed from all notifications for {0}", this.CommandSource)
            };
        }

        [SubcommandInvocation("subscribe")]
        [Help("", "Subscribe to email notifications from all stalks in this channel")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> SubscribeMode()
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
            
            
            if (!this.subscriptionHelper.SubscribeChannel(botUser.Mask, this.channelConfiguration[this.CommandSource]))
            {
                yield return new CommandResponse
                {
                    Message = string.Format("You are already subscribed to notifications for {0}", this.CommandSource)
                };

                yield break;
            }

            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format("Subscribed to all notifications for {0}", this.CommandSource)
            };
        }

        [SubcommandInvocation("part")]
        [CommandFlag(AccessFlags.GlobalAdmin, true)]
        [CommandFlag(AccessFlags.LocalAdmin)]
        [RequiredArguments(1)]
        [Help("<channel>", "Requests the bot leaves a channel, removing all configuration for that channel.")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> PartMode()
        {
            var tokenList = this.Arguments;
            var channel = tokenList.First();
            
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

        [SubcommandInvocation("join")]
        [CommandFlag(AccessFlags.GlobalAdmin, true)]
        [CommandFlag(AccessFlags.LocalAdmin)]
        [RequiredArguments(1)]
        [Help("<channel>", "Requests the bot joins a channel")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> JoinMode()
        {
            var tokenList = this.Arguments;
            var channel = tokenList.First();
            
            this.channelConfiguration.Add(new IrcChannel(channel));
            this.channelConfiguration.Save();
            this.Client.JoinChannel(channel);
            this.Client.SendMessage(
                channel,
                string.Format("My presence in {0} was requested by {1}.", channel, this.User));

            yield break;
        }
    }
}