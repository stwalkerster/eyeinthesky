namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;

    using Stwalkerster.IrcClient.Model;

    public class ChannelInfoModel : ModelBase
    {
        public IIrcChannel IrcChannel { get; set; }
        public List<IrcChannelUser> ChannelMembers { get; set; }
    }
}   