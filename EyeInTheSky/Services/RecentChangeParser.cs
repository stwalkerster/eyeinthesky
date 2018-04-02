namespace EyeInTheSky.Services
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class RecentChangeParser : IRecentChangeParser
    {
        private readonly ILogger logger;
        private readonly IMediaWikiApi mediaWikiApi;
        private const string FullStringRegex = @"14\[\[07(?<title>.*)14\]\]4 (?<flag>.*)10 02(?<url>[^ ]*) 5\* 03(?<user>.*) 5\* (?:\((?<szdiff>.*)\))? 10(?<comment>.*?)?$";
        private const string AntiColourParse = @"[01]?[0-9]";
        
        private Regex dataregex;
        private Regex colsregex;

        public RecentChangeParser(ILogger logger, IMediaWikiApi mediaWikiApi)
        {
            this.logger = logger;
            this.mediaWikiApi = mediaWikiApi;
        }

        private Regex GetRegex()
        {
            return this.dataregex ?? (this.dataregex = new Regex(FullStringRegex));
        }

        private Regex GetColourRegex()
        {
            return this.colsregex ?? (this.colsregex = new Regex(AntiColourParse));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public IRecentChange Parse(string data)
        {
            Match m = this.GetRegex().Match(data);
            if (m.Success == false)
            {
                throw new FormatException("Unable to match recent change against RC regex.");
            }

            var userValue = m.Groups["user"].Value;
            var urlValue = m.Groups["url"].Value;
            var flagValue = m.Groups["flag"].Value;
            var titleValue = m.Groups["title"].Value;

            var comment = m.Groups["comment"].Value;
            if (this.GetColourRegex().Match(comment).Success)
            {
                comment = this.GetColourRegex().Replace(comment, "");
            }

            int? rcSizeDiff = null;

            try
            {
                rcSizeDiff = m.Groups["szdiff"].Value == "" ? 0 : int.Parse(m.Groups["szdiff"].Value.Trim(''));
            }
            catch(FormatException ex)
            {
                this.logger.ErrorFormat(ex, "Can't parse size difference from RC: {0}", data);
            }

            var rc = new RecentChange(userValue);

            if (titleValue.StartsWith("Special:Log/"))
            {
                this.ConstructLogObject(rc, titleValue, comment, flagValue);
            }
            else
            {
                this.ConstructEditObject(rc, titleValue, urlValue, comment, flagValue, rcSizeDiff);
            }

            rc.MediaWikiApi = this.mediaWikiApi;
            return rc;
        }

        private void ConstructLogObject(RecentChange rc, string titleValue, string comment, string flagValue)
        {
            rc.Log = titleValue.Split('/').Skip(1).First();
            rc.EditFlags = flagValue;

            bool handled = false;

            switch (rc.Log)
            {
                case "block":
                    // TODO: capture indefinite block
                    // TODO: capture multiple block flags
                    // TODO: unblock
                    // TODO: parse expiry

                    if (rc.EditFlags == "block")
                    {
                        var match = new Regex("^blocked User:(?<targetUser>.*?) \\((?<flags>.*?)\\) with an expiry time of (?<expiry>.*?): (?<comment>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;
                            rc.EditSummary = result.Groups["comment"].Value;
                            rc.EditFlags += ", " + result.Groups["flags"].Value;

                            handled = true;
                        }
                    }

                    break;
                case "delete":
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex("^deleted \"\\[\\[(?<pageName>.*?)\\]\\]\": (?<comment>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;
                            rc.EditSummary = result.Groups["comment"].Value;

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "delete_redir")
                    {
                        var match = new Regex(@"deleted redirect \[\[(?<pageName>.*?)\]\] by overwriting: (?<comment>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;
                            rc.EditSummary = result.Groups["comment"].Value;

                            handled = true;
                        }
                    }
                    break;
                case "move":
                    if (rc.EditFlags == "move")
                    {
                        // TODO: move with comment
                        var match = new Regex("^moved \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<newpage>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;

                            handled = true;
                        }
                    }
                    if (rc.EditFlags == "move_redir")
                    {
                        // TODO: move without comment
                        var match = new Regex("^moved \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<newpage>.*?)\\]\\] over redirect: (?<comment>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;
                            rc.EditSummary = result.Groups["comment"].Value;

                            handled = true;
                        }
                    }
                    break;
                case "massmessage":
                    if (rc.EditFlags == "skipnouser")
                    {
                        var match = new Regex("^Delivery of \"(?<page>.*?)\" to \\[\\[User talk:(?<targetuser>.*?)\\]\\] was skipped because the user account does not exist$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetUser = result.Groups["targetuser"].Value;

                            handled = true;
                        }
                    }
                    break;
                case "newusers":
                    rc.TargetUser = rc.User;

                    if (rc.EditFlags == "create")
                    {
                        handled = true;
                    }

                    if (rc.EditFlags == "create2")
                    {
                        rc.TargetUser = comment.Substring("created new account User:".Length);
                        handled = true;
                    }
                    break;
                case "pagetriage-curation":
                    if (rc.EditFlags == "reviewed")
                    {
                        var match = new Regex("marked \\[\\[(?<page>.*?)\\]\\] as reviewed$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "thanks":
                    if (rc.EditFlags == "thank")
                    {
                        var match = new Regex("thanked (?<user>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["user"].Value;
                            handled = true;
                        }
                    }
                    
                    break;
            }

            if (!handled)
            {
                this.logger.ErrorFormat("Unhandled log entry of type {0} / {1}: {2}", rc.Log, rc.EditFlags, comment);
            }
        }

        private void ConstructEditObject(RecentChange rc, string titleValue, string urlValue, string comment,
            string flagValue, int? rcSizeDiff)
        {
            rc.Page = titleValue;
            if (!string.IsNullOrWhiteSpace(urlValue))
            {
                rc.Url = urlValue;
            }

            if (!string.IsNullOrWhiteSpace(comment))
            {
                rc.EditSummary = comment;
            }

            if (!string.IsNullOrWhiteSpace(flagValue))
            {
                rc.EditFlags = flagValue;
            }

            if (rcSizeDiff.HasValue)
            {
                rc.SizeDiff = rcSizeDiff;
            }
        }
    }
}