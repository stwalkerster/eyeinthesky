namespace EyeInTheSky.Web.Modules
{
    using System.Linq;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Models;
    using Stwalkerster.IrcClient.Interfaces;

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
            this.Get["/channel/{channel}"] = this.GetChannelInfo;
            this.Get["/channel/{channel}/stalk/{stalk}"] = this.GetStalkInfo;
        }

        public dynamic GetChannelList(dynamic parameters)
        {
            var getChannelListModel = this.CreateModel<GetChannelListModel>(this.Context);
            getChannelListModel.Channels = this.channelConfiguration.Items.ToList();

            return getChannelListModel;
        }
        
        public dynamic GetChannelInfo(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public dynamic GetStalkInfo(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}