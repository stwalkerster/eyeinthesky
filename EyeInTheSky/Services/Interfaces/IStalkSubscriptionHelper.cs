namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

    using Stwalkerster.IrcClient.Model;

    public interface IStalkSubscriptionHelper
    {
        bool SubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
        bool UnsubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
        bool IsSubscribedToStalk(BotUser botUser, IIrcChannel channel, IStalk stalk);
        IEnumerable<StalkSubscriptionHelper.SubscriptionResult> GetUserSubscriptionsToStalk(IIrcChannel channel, IStalk stalk);
        IEnumerable<StalkSubscriptionHelper.SubscriptionResult> GetUserSubscriptionsInChannel(IBotUser user, IIrcChannel channel);
    }
}