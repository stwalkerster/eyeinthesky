namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    public class ModelBase
    {
        public IBotUser BotUser { get; set; }

        public IAppConfiguration AppConfiguration { get; set; }
        public IIrcClient IrcClient { get; set; }

        public List<string> Errors { get; set; }
        public string Version { get; set; }
    }
}