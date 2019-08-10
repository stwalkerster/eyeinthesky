namespace EyeInTheSky.Services.RecentChanges.Interfaces
{
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Events;

    public interface IRecentChangeHandler
    {
        void HandleRcEvent(MessageReceivedEventArgs e, IRecentChange rc);
    }
}