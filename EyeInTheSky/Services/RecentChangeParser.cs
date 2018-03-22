namespace EyeInTheSky.Services
{
    using System;
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

            var rcSizeDiff = 0;
            
            try
            {
                rcSizeDiff = m.Groups["szdiff"].Value == "" ? 0 : int.Parse(m.Groups["szdiff"].Value.Trim(''));
            }
            catch(FormatException ex)
            {
                this.logger.ErrorFormat(ex, "Can't parse size difference from RC: {0}", data);
            }
            
            var rc = new RecentChange(titleValue, userValue, urlValue, comment, flagValue, rcSizeDiff);

            rc.MediaWikiApi = this.mediaWikiApi;

            return rc;
        }
    }
}