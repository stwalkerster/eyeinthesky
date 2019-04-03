namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;

    public class MainPageModel : ModelBase
    {
        public IEnumerable<IStalk> SubscribedStalks { get; set; }
        public IEnumerable<IIrcChannel> SubscribedChannels { get; set; }
    }
}