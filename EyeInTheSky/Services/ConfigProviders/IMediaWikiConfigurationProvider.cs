namespace EyeInTheSky.Services.ConfigProviders
{
    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public interface IMediaWikiConfigurationProvider
    {
        IMediaWikiConfiguration GetConfigurationForChannel(string channel);
    }
}