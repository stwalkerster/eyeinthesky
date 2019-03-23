namespace EyeInTheSky.Services.RecentChanges.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Events;

    public interface IRecentChangeHandler
    {
        void HandleRcEvent(MessageReceivedEventArgs e, IRecentChange rc);
        string FormatStalkListForEmail(IEnumerable<IStalk> stalks, IBotUser botUser);
    }
}