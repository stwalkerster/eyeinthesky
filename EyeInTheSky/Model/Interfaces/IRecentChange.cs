namespace EyeInTheSky.Model.Interfaces
{
    public interface IRecentChange
    {
        string Page { get; }
        string User { get; }
        string Url { get; }
        string EditSummary { get; }
        string EditFlags { get; }
        int SizeDifference { get; }
    }
}