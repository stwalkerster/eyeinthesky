namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;

    public class MainPageModel : ModelBase
    {
        public IEnumerable<dynamic> SubscribedStalks { get; set; }
        public IEnumerable<IIrcChannel> SubscribedChannels { get; set; }
    }
}