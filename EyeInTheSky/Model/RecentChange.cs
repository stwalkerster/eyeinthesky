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

        protected bool Equals(RecentChange other)
        {
            return string.Equals(this.Page, other.Page) && string.Equals(this.User, other.User)
                                                        && string.Equals(this.Url, other.Url)
                                                        && string.Equals(this.EditSummary, other.EditSummary)
                                                        && string.Equals(this.EditFlags, other.EditFlags)
                                                        && this.SizeDifference == other.SizeDifference;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecentChange) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.Page != null ? this.Page.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.User != null ? this.User.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Url != null ? this.Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.EditSummary != null ? this.EditSummary.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.EditFlags != null ? this.EditFlags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.SizeDifference;
                return hashCode;
            }
        }
    }
}