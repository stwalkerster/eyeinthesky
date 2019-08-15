namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class SubscriptionHelper : ISubscriptionHelper
    {
        private readonly ILogger logger;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IChannelConfiguration channelConfiguration;

        public SubscriptionHelper(
            ILogger logger,
            IBotUserConfiguration botUserConfiguration,
            IChannelConfiguration channelConfiguration)
        {
            this.logger = logger;
            this.botUserConfiguration = botUserConfiguration;
            this.channelConfiguration = channelConfiguration;
        }

        public bool SubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source)
        {
            if (channel.Identifier != stalk.Channel)
            {
                throw new Exception("Mismatch between stalk channel and channel!");
            }

            var stalkSubscriber = stalk.Subscribers.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());
            var channelSubscriber = channel.Users.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());

            this.logger.DebugFormat(
                "Subscription request for {0} to {1} in {2}",
                mask,
                stalk.Identifier,
                channel.Identifier);

            if (stalkSubscriber != null)
            {
                if (stalkSubscriber.Subscribed)
                {
                    if (channelSubscriber != null)
                    {
                        if (channelSubscriber.Subscribed)
                        {
                            // subscribed to channel
                            // subscribed to stalk
                            this.logger.WarnFormat(
                                "Found subscription request from stalk- ({0}) and channel-subscribed ({1}) user ({2})",
                                stalk.Identifier,
                                channel.Identifier,
                                mask);

                            this.logger.DebugFormat(
                                "Unsubscribing from stalk - already subscribed to stalk and channel");
                            stalk.Subscribers.Remove(stalkSubscriber);
                            source = SubscriptionSource.Channel;
                            return false;
                        }
                        else
                        {
                            // not subscribed to channel
                            // subscribed to stalk
                            this.logger.DebugFormat("Not subscribing - already subscribed to stalk");
                            source = SubscriptionSource.Stalk;
                            return false;
                        }
                    }
                    else
                    {
                        // not subscribed to channel
                        // subscribed to stalk
                        this.logger.DebugFormat("Not subscribing - already subscribed to stalk");
                        source = SubscriptionSource.Stalk;
                        return false;
                    }
                }
                else
                {
                    if (channelSubscriber != null)
                    {
                        if (channelSubscriber.Subscribed)
                        {
                            // forcibly unsubscribed from stalk
                            // subscribed to channel
                            this.logger.DebugFormat("Removing forced unsubscribe - already subscribed to channel");
                            stalk.Subscribers.Remove(stalkSubscriber);
                            source = SubscriptionSource.Channel;
                            return true;
                        }
                        else
                        {
                            // forcibly unsubscribed from stalk
                            // not subscribed to channel
                            this.logger.WarnFormat(
                                "Found subscription request from stalk-force-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})",
                                stalk.Identifier,
                                channel.Identifier,
                                mask);
                            this.logger.DebugFormat("Converting forced unsubscribe to stalk subscription");
                            stalkSubscriber.Subscribed = true;
                            source = SubscriptionSource.Stalk;
                            return true;
                        }
                    }
                    else
                    {
                        // forcibly unsubscribed from stalk
                        // not subscribed to channel
                        this.logger.WarnFormat(
                            "Found subscription request from stalk-force-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})",
                            stalk.Identifier,
                            channel.Identifier,
                            mask);
                        this.logger.DebugFormat("Converting forced unsubscribe to stalk subscription");
                        stalkSubscriber.Subscribed = true;
                        source = SubscriptionSource.Stalk;
                        return true;
                    }
                }
            }
            else
            {
                if (channelSubscriber != null)
                {
                    if (channelSubscriber.Subscribed)
                    {
                        // already subscribed to channel
                        // not subscribed to stalk
                        source = SubscriptionSource.Channel;
                        return false;
                    }
                    else
                    {
                        // not subscribed to channel
                        // not subscribed to stalk
                        this.logger.DebugFormat("Subscribing to stalk");
                        stalkSubscriber = new StalkUser(mask, true);
                        stalk.Subscribers.Add(stalkSubscriber);
                        source = SubscriptionSource.Stalk;
                        return true;
                    }
                }
                else
                {
                    // not subscribed to channel
                    // not subscribed to stalk
                    this.logger.DebugFormat("Subscribing to stalk");
                    stalkSubscriber = new StalkUser(mask, true);
                    stalk.Subscribers.Add(stalkSubscriber);
                    source = SubscriptionSource.Stalk;
                    return true;
                }
            }
        }

        public bool UnsubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source)
        {
            if (channel.Identifier != stalk.Channel)
            {
                throw new Exception("Mismatch between stalk channel and channel!");
            }

            var stalkSubscriber = stalk.Subscribers.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());
            var channelSubscriber = channel.Users.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());

            this.logger.DebugFormat(
                "Unsubscription request for {0} to {1} in {2}",
                mask,
                stalk.Identifier,
                channel.Identifier);

            if (stalkSubscriber != null)
            {
                if (stalkSubscriber.Subscribed)
                {
                    if (channelSubscriber != null)
                    {
                        if (channelSubscriber.Subscribed)
                        {
                            // subscribed to channel
                            // subscribed to stalk
                            this.logger.WarnFormat(
                                "Found unsubscription request from stalk- ({0}) and channel-subscribed ({1}) user ({2})",
                                stalk.Identifier,
                                channel.Identifier,
                                mask);
                            this.logger.DebugFormat(
                                "Forcing unsubscribe from stalk - already subscribed to stalk and channel");
                            stalkSubscriber.Subscribed = false;
                            source = SubscriptionSource.Stalk;
                            return true;
                        }
                        else
                        {
                            // not subscribed to channel
                            // subscribed to stalk
                            this.logger.DebugFormat("Unsubscribing from stalk");
                            stalk.Subscribers.Remove(stalkSubscriber);
                            source = SubscriptionSource.Stalk;
                            return true;
                        }
                    }
                    else
                    {
                        // not subscribed to channel
                        // subscribed to stalk
                        this.logger.DebugFormat("Unsubscribing from stalk");
                        stalk.Subscribers.Remove(stalkSubscriber);
                        source = SubscriptionSource.Stalk;
                        return true;
                    }
                }
                else
                {
                    if (channelSubscriber != null)
                    {
                        if (channelSubscriber.Subscribed)
                        {
                            // forcibly unsubscribed from stalk
                            // subscribed to channel
                            this.logger.DebugFormat("Already forcibly unsubscribed from stalk");
                            source = SubscriptionSource.Stalk;
                            return false;
                        }
                        else
                        {
                            // forcibly unsubscribed from stalk
                            // not subscribed to channel
                            this.logger.WarnFormat(
                                "Found unsubscription request from stalk-forcibly-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})",
                                stalk.Identifier,
                                channel.Identifier,
                                mask);
                            this.logger.DebugFormat("Removing stalk subscription");
                            stalk.Subscribers.Remove(stalkSubscriber);
                            source = SubscriptionSource.Stalk;
                            return false;
                        }
                    }
                    else
                    {
                        // forcibly unsubscribed from stalk
                        // not subscribed to channel
                        this.logger.WarnFormat(
                            "Found unsubscription request from stalk-forcibly-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})",
                            stalk.Identifier,
                            channel.Identifier,
                            mask);
                        this.logger.DebugFormat("Removing stalk subscription");
                        stalk.Subscribers.Remove(stalkSubscriber);
                        source = SubscriptionSource.Stalk;
                        return false;
                    }
                }
            }
            else
            {
                if (channelSubscriber != null)
                {
                    if (channelSubscriber.Subscribed)
                    {
                        // already subscribed to channel
                        // not subscribed to stalk
                        this.logger.DebugFormat("Forcing unsubscribe");
                        stalkSubscriber = new StalkUser(mask, false);
                        stalk.Subscribers.Add(stalkSubscriber);
                        source = SubscriptionSource.Stalk;
                        return true;
                    }
                    else
                    {
                        // not subscribed to channel
                        // not subscribed to stalk
                        this.logger.DebugFormat("Already not subscribed!");
                        source = SubscriptionSource.None;
                        return false;
                    }
                }
                else
                {
                    // not subscribed to channel
                    // not subscribed to stalk
                    this.logger.DebugFormat("Already not subscribed!");
                    source = SubscriptionSource.None;
                    return false;
                }
            }
        }

        public bool IsSubscribedToStalk(IBotUser botUser, IIrcChannel channel, IStalk stalk)
        {
            return this.GetUserSubscriptionsToStalk(channel, stalk)
                .Where(x => x.IsSubscribed)
                .Any(x => Equals(x.BotUser, botUser));
        }

        public bool SubscribeChannel(IrcUserMask mask, IIrcChannel channel)
        {
            var channelUser = channel.Users.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());
            if (channelUser == null)
            {
                channelUser = new ChannelUser(mask);
                channel.Users.Add(channelUser);
            }

            if (channelUser.Subscribed)
            {
                return false;
            }
            else
            {
                channelUser.Subscribed = true;

                // remove any overrides
                var channelSubscriptions = this.GetUserStalkSubscriptionsInChannel(new BotUser(mask), channel);
                foreach (var subscriptionResult in channelSubscriptions.Where(x => x.Source == SubscriptionSource.Stalk))
                {
                    subscriptionResult.Stalk.Subscribers.RemoveAll(x => x.Mask.Equals(mask));
                }

                return true;
            }
        }

        public bool UnsubscribeChannel(IrcUserMask mask, IIrcChannel channel)
        {
            var channelUser = channel.Users.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());

            if (channelUser == null)
            {
                return false;
            }

            var result = channelUser.Subscribed;
            channelUser.Subscribed = false;
            
            // remove any overrides
            var channelSubscriptions = this.GetUserStalkSubscriptionsInChannel(new BotUser(mask), channel);
            foreach (var subscriptionResult in channelSubscriptions.Where(x => x.Source == SubscriptionSource.Stalk))
            {
                subscriptionResult.Stalk.Subscribers.RemoveAll(x => x.Mask.Equals(mask));
            }

            return result;
        }

        public bool IsSubscribedToChannel(IBotUser botUser, IIrcChannel channel)
        {
            return this.GetUserSubscriptionsToChannel(botUser).Any(x => x.Equals(channel));
        }

        public IEnumerable<IIrcChannel> GetUserSubscriptionsToChannel(IBotUser botUser)
        {
            return this.channelConfiguration.Items
                .Where(channel => channel.Users.Select(y => y.Mask).Contains(botUser.Mask))
                .Where(channel => channel.Users.First(x => x.Mask.Equals(botUser.Mask)).Subscribed)
                .ToList();
        }

        public IEnumerable<SubscriptionResult> GetUserSubscriptionsToStalk(IIrcChannel channel, IStalk stalk)
        {
            var userData = this.botUserConfiguration.Items.ToDictionary(
                x => x,
                y => new SubscriptionResult {Stalk = stalk, Channel = channel, BotUser = y});

            foreach (var channelUser in channel.Users.Where(x => x.Subscribed))
            {
                var botUser = this.botUserConfiguration[channelUser.Mask.ToString()];
                userData[botUser].IsSubscribed = true;
                userData[botUser].Source = SubscriptionSource.Channel;
                userData[botUser].Complete = true;
            }

            foreach (var stalkUser in stalk.Subscribers)
            {
                var botUser = this.botUserConfiguration[stalkUser.Mask.ToString()];

                if (stalkUser.Subscribed)
                {
                    userData[botUser].IsSubscribed = true;
                    userData[botUser].Overridden = false;
                    userData[botUser].Source = SubscriptionSource.Stalk;
                    userData[botUser].Complete = true;
                }
                else
                {
                    // subscription exclusion for channel users
                    userData[botUser].IsSubscribed = false;
                    userData[botUser].Overridden = true;
                    userData[botUser].Source = SubscriptionSource.Stalk;
                    userData[botUser].Complete = true;
                }
            }

            return userData.Where(x => x.Value.Complete).Select(x => x.Value).ToList();
        }

        public IEnumerable<SubscriptionResult> GetUserStalkSubscriptionsInChannel(IBotUser user, IIrcChannel channel)
        {
            var channelSubscribed = channel.Users.Where(x => x.Subscribed).Any(x => x.Mask.Equals(user.Mask));

            var results = new List<SubscriptionResult>();

            foreach (var stalk in channel.Stalks)
            {
                var result = new SubscriptionResult
                {
                    Channel = channel, BotUser = user, Stalk = stalk.Value, Complete = true
                };

                if (channelSubscribed)
                {
                    result.IsSubscribed = true;
                    result.Source = SubscriptionSource.Channel;
                }

                var subscription = stalk.Value.Subscribers.FirstOrDefault(x => x.Mask.Equals(user.Mask));
                if (subscription == null)
                {
                    // use the channel result.
                    results.Add(result);
                    continue;
                }

                if (subscription.Subscribed)
                {
                    result.IsSubscribed = true;
                    result.Source = SubscriptionSource.Stalk;
                }
                else
                {
                    result.IsSubscribed = false;
                    result.Overridden = true;
                    result.Source = SubscriptionSource.Stalk;
                }

                results.Add(result);
            }

            return results;
        }

        public sealed class SubscriptionResult
        {
            internal SubscriptionResult()
            {
            }

            public IStalk Stalk { get; internal set; }
            public IIrcChannel Channel { get; internal set; }
            public IBotUser BotUser { get; internal set; }
            public bool IsSubscribed { get; internal set; }
            public SubscriptionSource Source { get; internal set; }
            public bool Overridden { get; internal set; }

            // has this item been completely constructed?
            internal bool Complete { get; set; }
        }
    }
}