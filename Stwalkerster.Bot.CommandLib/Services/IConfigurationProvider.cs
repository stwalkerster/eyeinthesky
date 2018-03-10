namespace Stwalkerster.Bot.CommandLib.Services
{
    public interface IConfigurationProvider
    {
        string CommandTrigger { get; }
        string DebugChannel { get; }
    }
}