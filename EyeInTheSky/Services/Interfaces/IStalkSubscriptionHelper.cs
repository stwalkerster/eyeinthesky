namespace EyeInTheSky.Services.Interfaces
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

    using Stwalkerster.IrcClient.Model;

    public interface IStalkSubscriptionHelper
    {
        bool SubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
        bool UnsubscribeStalk(IrcUserMask mask, IIrcChannel channel, IStalk stalk, out SubscriptionSource source);
    }
}