namespace EyeInTheSky.Model
{
    using EyeInTheSky.Model.Interfaces;
    public class RecentChange : IRecentChange
    {
        public RecentChange(string page, string user, string url, string editsummary, string flags, int sizediff)
        {
            this.Page = page;
            this.User = user;
            this.Url = url;
            this.EditSummary = editsummary;
            this.EditFlags = flags;
            this.SizeDifference = sizediff;
        }

        public string Page { get; private set; }

        public string User { get; private set; }

        public string Url { get; private set; }

        public string EditSummary { get; private set; }

        public string EditFlags { get; private set; }

        public int SizeDifference { get; private set; }
    }
}