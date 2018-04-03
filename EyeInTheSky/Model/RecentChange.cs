namespace EyeInTheSky.Model
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class RecentChange : IRecentChange
    {
        private IEnumerable<string> usergroups;

        public RecentChange(string user)
        {
            this.User = user;
        }

        public string Page { get; set; }

        public string TargetPage { get; set; }

        public string User { get; set; }

        public string TargetUser { get; set; }

        public string Url { get; set; }

        public string EditSummary { get; set; }

        public string EditFlags { get; set; }

        public int? SizeDiff { get; set; }

        public string Log { get; set; }

        public TimeSpan? Expiry { get; set; }

        public string AdditionalData { get; set; }

        public IMediaWikiApi MediaWikiApi { get; set; }

        public IEnumerable<string> GetUserGroups()
        {
            if (this.MediaWikiApi == null)
            {
                throw new InvalidOperationException("API helper not available");
            }

            if (this.usergroups == null)
            {
                this.usergroups = this.MediaWikiApi.GetUserGroups(this.User);
            }
            
            return this.usergroups;
        }

        public bool PageIsInCategory(string category)
        {
            if (this.MediaWikiApi == null)
            {
                throw new InvalidOperationException("API helper not available");
            }
            
            if (this.Page == null)
            {
                return false;
            }

            if (!category.StartsWith("Category:"))
            {
                category = "Category:" + category;
            }
            
            return this.MediaWikiApi.PageIsInCategory(this.Page, category);
        }

        protected bool Equals(RecentChange other)
        {
            return string.Equals(this.Page, other.Page) && string.Equals(this.TargetPage, other.TargetPage) &&
                   string.Equals(this.User, other.User) && string.Equals(this.TargetUser, other.TargetUser) &&
                   string.Equals(this.Url, other.Url) && string.Equals(this.EditSummary, other.EditSummary) &&
                   string.Equals(this.EditFlags, other.EditFlags) && this.SizeDiff == other.SizeDiff &&
                   string.Equals(this.Log, other.Log) && string.Equals(this.AdditionalData, other.AdditionalData);
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
                hashCode = (hashCode * 397) ^ (this.TargetPage != null ? this.TargetPage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.User != null ? this.User.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.TargetUser != null ? this.TargetUser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Url != null ? this.Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.EditSummary != null ? this.EditSummary.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.EditFlags != null ? this.EditFlags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.SizeDiff.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Log != null ? this.Log.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.AdditionalData != null ? this.AdditionalData.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}