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
        private readonly IChannelConfiguration channelConfiguration;
        private readonly ISubscriptionHelper subscriptionHelper;

        public DefaultModule(
            IChannelConfiguration channelConfiguration,
            ISubscriptionHelper subscriptionHelper,
            IAppConfiguration appConfiguration,
            IIrcClient client) : base(appConfiguration, client)
        {
            this.channelConfiguration = channelConfiguration;
            this.subscriptionHelper = subscriptionHelper;
            this.Get["/"] = this.MainPage;
        }

        public dynamic MainPage(dynamic parameters)
        {
            var model = this.CreateModel<MainPageModel>(this.Context);

            var botUser = ((UserIdentity) this.Context.CurrentUser).BotUser;
            var mask = botUser.Mask;

            var subscribedChannels = this.subscriptionHelper.GetUserSubscriptionsToChannel(botUser);

            var unsubscribedChannels =
                this.channelConfiguration.Items.Where(x => !subscribedChannels.Any(y => x.Equals(y)));


            var subscribedStalks =
                unsubscribedChannels
                .Aggregate(new List<dynamic>(), (list, channel) =>
                {
                    var subscriptions = this.subscriptionHelper.GetUserStalkSubscriptionsInChannel(botUser, channel)
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