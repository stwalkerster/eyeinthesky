namespace Stwalkerster.Bot.CommandLib.Services
{
    using System.Collections.Generic;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public interface IFlagService
    {
        bool UserHasFlag(IUser user, string flag);
        IEnumerable<string> GetFlagsForUser(IUser user);
    }
}