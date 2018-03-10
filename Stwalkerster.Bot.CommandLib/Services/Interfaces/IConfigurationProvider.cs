namespace Stwalkerster.Bot.CommandLib.Services.Interfaces
{
    public interface IConfigurationProvider
    {
        string CommandPrefix { get; }
        string DebugChannel { get; }
    }
}