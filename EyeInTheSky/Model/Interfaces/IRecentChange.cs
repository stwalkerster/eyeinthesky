namespace EyeInTheSky.Model.Interfaces
{
    using System.Collections.Generic;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    public interface IRecentChange
    {
        string Page { get; }
        string User { get; }
        string Url { get; }
        string EditSummary { get; }
        string EditFlags { get; }
        string Log { get; }
        IMediaWikiApi MediaWikiApi { get; set; }
        string TargetPage { get; set; }
        string TargetUser { get; set; }
        int? SizeDiff { get; set; }
        string AdditionalData { get; set; }
        string AlternateTargetUser { get; set; }

        IEnumerable<string> GetUserGroups();
        bool PageIsInCategory(string category);
        long GetPageSize();
    }
}