namespace EyeInTheSky.Services.ConfigProviders
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public class PatternMediaWikiConfigProvider : IMediaWikiConfigurationProvider
    {
        private readonly string urlReplacement;
        private readonly string userAgent;
        private readonly Regex channelMatch;

        private readonly Dictionary<string, IMediaWikiConfiguration> cache = new Dictionary<string, IMediaWikiConfiguration>();
        
        public PatternMediaWikiConfigProvider(string channelMatch, string urlReplacement, string userAgent)
        {
            this.urlReplacement = urlReplacement;
            this.userAgent = userAgent;
            this.channelMatch = new Regex(channelMatch);
        }
        
        public IMediaWikiConfiguration GetConfigurationForChannel(string channel)
        {
            lock (this.cache)
            {
                if (this.cache.ContainsKey(channel))
                {
                    return this.cache[channel];
                }

                var config = new MediaWikiConfiguration(this.channelMatch.Replace(channel, this.urlReplacement),
                    this.userAgent);
                
                this.cache.Add(channel, config);

                return config;
            }
        }
    }
}