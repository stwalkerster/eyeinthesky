namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;

    using Nancy;

    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class ChannelModule : AuthenticatedModuleBase
    {
        private readonly IChannelConfiguration channelConfiguration;

        public ChannelModule(
            IAppConfiguration appConfiguration,
            IIrcClient freenodeClient,
            IChannelConfiguration channelConfiguration) : base(appConfiguration, freenodeClient)
        {
            this.channelConfiguration = channelConfiguration;

            this.Get["/channels"] = this.GetChannelList;
            this.Get["/channels/{channel}"] = this.GetChannelInfo;
            this.Get["/channels/{channel}/stalk/{stalk}"] = this.GetStalkInfo;
        }

        public dynamic GetChannelList(dynamic parameters)
        {
            var getChannelListModel = this.CreateModel<GetChannelListModel>(this.Context);
            getChannelListModel.Channels = this.channelConfiguration.Items.ToList();

            return getChannelListModel;
        }

        public dynamic GetChannelInfo(dynamic parameters)
        {
            var channel = this.channelConfiguration.Items.FirstOrDefault(x => x.Guid == parameters.channel);

            if (channel == null)
            {
                return new NotFoundResponse();
            }

            var model = this.CreateModel<ChannelInfoModel>(this.Context);
            model.IrcChannel = channel;

            model.Stalks = channel.Stalks.Values.Select(v => new DisplayStalk(v, this.AppConfiguration)).ToList();

            if (model.IrcClient.Channels.ContainsKey(channel.Identifier))
            {
                var ircClientChannel = model.IrcClient.Channels[channel.Identifier];

                model.ChannelMembers = ircClientChannel.Users.Values.ToList();
            }
            else
            {
                model.ChannelMembers = new List<IrcChannelUser>();
                model.Errors.Add(string.Format("{0} is not currently in this channel, or tracking is broken.",
                    model.IrcClient.Nickname));
            }

            return model;
        }

        public dynamic GetStalkInfo(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}