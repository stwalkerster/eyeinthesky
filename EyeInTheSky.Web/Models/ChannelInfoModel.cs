namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Web.Misc;
    using Stwalkerster.IrcClient.Model;

    public class ChannelInfoModel : ModelBase
    {
        public IIrcChannel IrcChannel { get; set; }
        public List<IrcChannelUser> ChannelMembers { get; set; }

        public List<DisplayStalk> Stalks { get; set; }

        public List<ChannelDisplayUser> DisplayUsers { get; set; }

        public bool ShowChannelSubscribeButton
        {
            get { return this.CanSeeChannelConfig && !this.IsChannelSubscribed; }
        }
        
        public bool ShowChannelUnsubscribeButton
        {
            get { return this.CanSeeChannelConfig && this.IsChannelSubscribed; }
        }

        public bool ShowNoStalksDefined
        {
            get { return this.CanSeeChannelConfig && this.Stalks.Count == 0; }
        }

        public bool CanSeeChannelConfig { get; set; }
        public bool CanConfigureStalks { get; set; }
        public bool IsChannelSubscribed { get; set; }
    }
}
