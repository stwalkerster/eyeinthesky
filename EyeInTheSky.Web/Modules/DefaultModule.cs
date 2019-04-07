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

        public DefaultModule(
            IBotUserConfiguration botUserConfiguration,
            IChannelConfiguration channelConfiguration,
            IAppConfiguration appConfiguration,
            IIrcClient client) : base(appConfiguration, client)
        {
            this.botUserConfiguration = botUserConfiguration;
            this.channelConfiguration = channelConfiguration;
            this.Get["/"] = this.MainPage;
        }

        public dynamic MainPage(dynamic parameters)
        {
            var model = this.CreateModel<MainPageModel>(this.Context);

            var botUser = ((UserIdentity) this.Context.CurrentUser).BotUser;
            var mask = botUser.Mask;

            var subscribedChannels = this.channelConfiguration.Items
                .Where(channel => channel.Users.Select(y => y.Mask).Contains(mask))
                .ToList();

            var subscribedStalks = this.channelConfiguration.Items
                .Where(x => !x.Users.Select(channelUser => channelUser.Mask).Contains(mask))
                .Aggregate(new List<IStalk>(), (list, channel) =>
                {
                    list.AddRange(channel.Stalks.Values.Where(x =>
                        x.Subscribers.Where(stalkUser => stalkUser.Subscribed).Select(stalkUser => stalkUser.Mask)
                            .Contains(mask)));
                    return list;
                })
                .ToList();

            model.SubscribedChannels = subscribedChannels;
            model.SubscribedStalks = subscribedStalks;

            return model;
        }
    }
}