namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    public class ModelBase
    {
        // used from Nancy
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IBotUser BotUser { get; set; }

        // used from Nancy
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IAppConfiguration AppConfiguration { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // used from Nancy
        public IIrcClient IrcClient { get; set; }

        // ReSharper disable once CollectionNeverQueried.Global
        // used from Nancy
        public List<string> Errors { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        //used from nancy
        public string Version { get; set; }
    }
}