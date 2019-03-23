namespace EyeInTheSky.Services.RecentChanges.Irc.Interfaces
{
    using EyeInTheSky.Model.Interfaces;

    public interface IIrcRecentChangeParser
    {
        IRecentChange Parse(string data, string channel);
    }
}