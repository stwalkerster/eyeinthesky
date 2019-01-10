namespace EyeInTheSky.Services
{
    using EyeInTheSky.Services.ConfigProviders;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.TypedFactories;

    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public class MediaWikiApiHelper : IMediaWikiApiHelper
    {
        private readonly IMediaWikiConfigurationProvider configurationProvider;
        private readonly IMediaWikiApiTypedFactory factory;

        public MediaWikiApiHelper(IMediaWikiConfigurationProvider configurationProvider, IMediaWikiApiTypedFactory factory)
        {
            this.configurationProvider = configurationProvider;
            this.factory = factory;
        }

        public IMediaWikiApi GetApiForChannel(string channel)
        {
            var config = this.configurationProvider.GetConfigurationForChannel(channel);
            var api = this.factory.Create<IMediaWikiApi>(config);

            return api;
        }

        public void Release(IMediaWikiApi api)
        {
            this.factory.Release(api);
        }
    }
}