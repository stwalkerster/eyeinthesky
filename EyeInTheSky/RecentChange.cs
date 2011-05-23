using System;
using System.Text.RegularExpressions;

namespace EyeInTheSky
{
    public class RecentChange
    {
        const string fullstringregex = @"14\[\[07(?<title>.*)14\]\]4 (?<flag>.*)10 02(?<url>[^ ]*) 5\* 03(?<user>.*) 5\* (?:\((?<szdiff>.*)\))? 10(?<comment>.*)";
        private const string anticolourparse = @"[01]?[0-9]";

        private RecentChange()
        {
        }

        public RecentChange(string page, string user, string url, string editsummary, string flags, int sizediff)
        {
            title = page;
            this.user = user;
            this.url = url;
            comment = editsummary;
            flag = flags;
            szdiff = sizediff;
        }

        private static Regex dataregex;
        private static Regex colsregex;

        public string Page
        {
            get { return title; }
        }

        public string User
        {
            get { return user; }
        }

        public string Url
        {
            get { return url; }
        }

        public string EditSummary
        {
            get { return comment; }
        }

        public string EditFlags
        {
            get { return flag; }
        }

        public int SizeDifference
        {
            get { return szdiff; }
        }

        private static Regex getRegex()
        {
            return dataregex ?? (dataregex = new Regex(fullstringregex));
        }

        private static Regex getColourRegex()
        {
            return colsregex ?? (colsregex = new Regex(anticolourparse));
        }

        public static RecentChange parse(string data)
        {
            Match m = getRegex().Match(data);
            if (m.Success == false)
                throw new FormatException();

            RecentChange rc = new RecentChange
                                  {
                                      user = m.Groups["user"].Value,
                                      url = m.Groups["url"].Value,
                                      flag = m.Groups["flag"].Value,
                                      title = m.Groups["title"].Value
                                  };

            string comment = m.Groups["comment"].Value;
            if (getColourRegex().Match(comment).Success)
                comment = getColourRegex().Replace(comment, "");
            rc.comment = comment;

            try
            {
                rc.szdiff = m.Groups["szdiff"].Value == "" ? 0 : int.Parse(m.Groups["szdiff"].Value.Trim(''));
            }
            catch(FormatException)
            {
                rc.szdiff = 0;
            }

            return rc;
        }

        private string title;
        private string user;
        private string url;
        private string comment;
        private string flag;
        private int szdiff;
    }
}
