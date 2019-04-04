namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Web.Misc;
    using Stwalkerster.IrcClient.Model;

    public class ChannelInfoModel : ModelBase
    {
        public IIrcChannel IrcChannel { get; set; }
        public List<IrcChannelUser> ChannelMembers { get; set; }

        public List<DisplayStalk> Stalks { get; set; }
    }
}
