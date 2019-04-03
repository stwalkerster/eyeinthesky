namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public class GetChannelListModel : ModelBase
    {
        public List<IIrcChannel> Channels { get; set; }
    }
}