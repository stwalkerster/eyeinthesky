namespace EyeInTheSky.Services.ConfigProviders
{
    using System.Collections.Generic;
    using System.Linq;

    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public class MapMediaWikiConfigProvider : IMediaWikiConfigurationProvider
    {
        private readonly IList<MapEntry> channelMap;

        public MapMediaWikiConfigProvider(IList<MapEntry> channelMap)
        {
            this.channelMap = channelMap;
        }

        public class MapEntry
        {
            public string Channel { get; private set; }
            public MediaWikiConfiguration Configuration { get; private set; }

            public MapEntry(string channel, MediaWikiConfiguration configuration)
            {
                this.Channel = channel;
                this.Configuration = configuration;
            }
        }

        public IMediaWikiConfiguration GetConfigurationForChannel(string channel)
        {
            var config = this.channelMap.FirstOrDefault(x => x.Channel == channel);

            if (config == null)
            {
                return null;
            }
            
            return config.Configuration;
        }
    }
}