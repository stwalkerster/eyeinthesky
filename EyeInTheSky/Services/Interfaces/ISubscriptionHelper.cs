namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

    using Stwalkerster.IrcClient.Model;

    public interface ISubscriptionHelper
    {
        bool SubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
        bool UnsubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
        bool IsSubscribedToStalk(IBotUser botUser, IIrcChannel channel, IStalk stalk);
        IEnumerable<SubscriptionHelper.SubscriptionResult> GetUserSubscriptionsToStalk(IIrcChannel channel, IStalk stalk);
        IEnumerable<SubscriptionHelper.SubscriptionResult> GetUserStalkSubscriptionsInChannel(IBotUser user, IIrcChannel channel);
        IEnumerable<IIrcChannel> GetUserSubscriptionsToChannel(IBotUser botUser);
    }
}