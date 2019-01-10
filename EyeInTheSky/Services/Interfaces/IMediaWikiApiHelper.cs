namespace EyeInTheSky.Services.Interfaces
{
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public interface IMediaWikiApiHelper
    {
        IMediaWikiApi GetApiForChannel(string channel);
        void Release(IMediaWikiApi api);
    }
}