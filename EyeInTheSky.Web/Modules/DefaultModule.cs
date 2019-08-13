namespace EyeInTheSky.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
            this.Get["/about"] = this.AboutPage;
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

        public dynamic AboutPage(dynamic parameters)
        {
            var model = this.CreateModel<AboutModel>(this.Context);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToList();
            
            model.Other = assemblies
                .Where(
                    x => !x.FullName.StartsWith("Stwalkerster") && !x.FullName.StartsWith("EyeInTheSky")
                                                                && !x.FullName.StartsWith("System")
                                                                && !x.FullName.StartsWith("mscorlib")
                                                                && !x.FullName.StartsWith("Microsoft.CSharp"))
                .ToDictionary(
                    x => x.GetName().Name,
                    y => FileVersionInfo.GetVersionInfo(y.Location).ProductVersion);

            model.Core = assemblies
                .Where(x => x.FullName.StartsWith("Stwalkerster") || x.FullName.StartsWith("EyeInTheSky."))
                .ToDictionary(
                    x => x.GetName().Name,
                    y => FileVersionInfo.GetVersionInfo(y.Location).ProductVersion);
            
            return model;
        }
    }
}