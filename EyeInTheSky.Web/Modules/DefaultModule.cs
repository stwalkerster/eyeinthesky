namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using System.Linq;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;

    public class DefaultModule : AuthenticatedModuleBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IChannelConfiguration channelConfiguration;

        public DefaultModule(IBotUserConfiguration botUserConfiguration, IChannelConfiguration channelConfiguration)
        {
            this.botUserConfiguration = botUserConfiguration;
            this.channelConfiguration = channelConfiguration;
            this.Get["/"] = this.MainPage;
        }

        public dynamic MainPage(dynamic parameters)
        {
            var model = this.CreateModel<MainPageModel>(this.Context);

            var botUser = ((UserIdentity) this.Context.CurrentUser).BotUser;
            var mask = botUser.Mask.ToString();

            var subscribedChannels = this.channelConfiguration.Items
                .Where(channel => channel.Users.Select(y => y.Mask.ToString()).Contains(mask))
                .ToList();

            var subscribedStalks = this.channelConfiguration.Items
                .Where(x => !x.Users.Select(channelUser => channelUser.Mask.ToString()).Contains(mask))
                .Aggregate(new List<IStalk>(), (list, channel) =>
                {
                    list.AddRange(channel.Stalks.Values.Where(x =>
                        x.Subscribers.Where(stalkUser => stalkUser.Subscribed).Select(stalkUser => stalkUser.Mask.ToString())
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