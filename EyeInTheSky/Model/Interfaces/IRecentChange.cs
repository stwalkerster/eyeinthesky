namespace EyeInTheSky.Model.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Services.Interfaces;

    public interface IRecentChange
    {
        string Page { get; }
        string User { get; }
        string Url { get; }
        string EditSummary { get; }
        string EditFlags { get; }
        int SizeDifference { get; }
        IMediaWikiApi MediaWikiApi { get; set; }

        IEnumerable<string> GetUserGroups();
        bool PageIsInCategory(string category);
    }
}