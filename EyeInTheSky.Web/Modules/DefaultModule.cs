namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;
    using Stwalkerster.IrcClient.Interfaces;

    public class DefaultModule : AuthenticatedModuleBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IStalkSubscriptionHelper subscriptionHelper;

        public DefaultModule(
            IBotUserConfiguration botUserConfiguration,
            IChannelConfiguration channelConfiguration,
            IStalkSubscriptionHelper subscriptionHelper,
            IAppConfiguration appConfiguration,
            IIrcClient client) : base(appConfiguration, client)
        {
            this.botUserConfiguration = botUserConfiguration;
            this.channelConfiguration = channelConfiguration;
            this.subscriptionHelper = subscriptionHelper;
            this.Get["/"] = this.MainPage;
        }

        public dynamic MainPage(dynamic parameters)
        {
            var model = this.CreateModel<MainPageModel>(this.Context);

            var botUser = ((UserIdentity) this.Context.CurrentUser).BotUser;
            var mask = botUser.Mask;

            var subscribedChannels = this.channelConfiguration.Items
                .Where(channel => channel.Users.Select(y => y.Mask).Contains(mask))
                .Where(channel => channel.Users.First(x => x.Mask.Equals(mask)).Subscribed)
                .ToList();

            var unsubscribedChannels =
                this.channelConfiguration.Items.Where(x => !subscribedChannels.Any(y => x.Equals(y)));


            var subscribedStalks =
                unsubscribedChannels
                .Aggregate(new List<dynamic>(), (list, channel) =>
                {
                    var subscriptions = this.subscriptionHelper.GetUserSubscriptionsInChannel(botUser, channel)
                        .Where(x => x.IsSubscribed);

                    list.AddRange(subscriptions);
                    return list;
                })
                .ToList();

            model.SubscribedChannels = subscribedChannels;
            model.SubscribedStalks = subscribedStalks;

            return model;
        }
    }
}