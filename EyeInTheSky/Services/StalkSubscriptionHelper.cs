namespace EyeInTheSky.Services
{
    using System;
    using System.Linq;

    using Castle.Core.Logging;

    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    using Stwalkerster.IrcClient.Model;

    public class StalkSubscriptionHelper : IStalkSubscriptionHelper
    {
        private readonly ILogger logger;

        public StalkSubscriptionHelper(ILogger logger)
        {
            this.logger = logger;
        }

        public bool SubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source)
        {
            if (channel.Identifier != stalk.Channel)
            {
                throw new Exception("Mismatch between stalk channel and channel!");
            }

            var stalkSubscriber = stalk.Subscribers.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());
            var channelSubscriber = channel.Users.FirstOrDefault(x => x.Mask.ToString() == mask.ToString());

            this.logger.DebugFormat("Subscription request for {0} to {1} in {2}", mask, stalk.Identifier, channel.Identifier);
            
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
                            this.logger.WarnFormat("Found subscription request from stalk- ({0}) and channel-subscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
                            
                            this.logger.DebugFormat("Unsubscribing from stalk - already subscribed to stalk and channel");
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
                            this.logger.WarnFormat("Found subscription request from stalk-force-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
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
                        this.logger.WarnFormat("Found subscription request from stalk-force-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
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

            this.logger.DebugFormat("Unsubscription request for {0} to {1} in {2}", mask, stalk.Identifier, channel.Identifier);
            
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
                            this.logger.WarnFormat("Found unsubscription request from stalk- ({0}) and channel-subscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
                            this.logger.DebugFormat("Forcing unsubscribe from stalk - already subscribed to stalk and channel");
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
                            this.logger.WarnFormat("Found unsubscription request from stalk-forcibly-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
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
                        this.logger.WarnFormat("Found unsubscription request from stalk-forcibly-unsubscribed ({0}) and channel-unsubscribed ({1}) user ({2})", stalk.Identifier, channel.Identifier, mask);
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
    }
}