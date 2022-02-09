namespace EyeInTheSky.Services.RecentChanges.Irc
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges.Irc.Interfaces;
    using Prometheus;

    public class IrcRecentChangeParser : IIrcRecentChangeParser
    {
        private static readonly Histogram ParseDuration = Metrics
            .CreateHistogram(
                "eyeinthesky_rc_irc_parse_duration_seconds",
                "Histogram of IRC RC event parse durations.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.001, 2, 13),
                });

        
        private readonly ILogger logger;
        private readonly IMediaWikiApiHelper mediaWikiApiHelper;
        private const string FullStringRegex = @"14\[\[07(?<title>.*)14\]\]4 (?<flag>.*)10 02(?<url>[^ ]*) 5\* 03(?<user>.*) 5\* (?:\((?<szdiff>.*)\))? 10(?<comment>.*?)?$";
        private const string AntiColourParse = @"[01]?[0-9]";
        
        private Regex dataregex;
        private Regex colsregex;

        public IrcRecentChangeParser(ILogger logger, IMediaWikiApiHelper mediaWikiApiHelper)
        {
            this.logger = logger;
            this.mediaWikiApiHelper = mediaWikiApiHelper;
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
        /// <param name="data">The RC entry from IRC</param>
        /// <param name="channel">The channel the RC entry was picked up from</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public IRecentChange Parse(string data, string channel)
        {
            RecentChange rc;
            
            using (ParseDuration.NewTimer())
            {
                Match m = this.GetRegex().Match(data);
                if (m.Success == false)
                {
                    throw new FormatException("Unable to match recent change against RC regex.");
                }

                rc = new RecentChange(m.Groups["user"].Value);
                
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
                catch (FormatException ex)
                {
                    this.logger.ErrorFormat(ex, "Can't parse size difference from RC: {0}", data);
                }

                if (titleValue.StartsWith("Special:Log/"))
                {
                    this.ConstructLogObject(rc, titleValue, comment, flagValue, data, channel);
                }
                else
                {
                    this.ConstructEditObject(rc, titleValue, urlValue, comment, flagValue, rcSizeDiff);
                }
            }

            rc.MediaWikiApi = this.mediaWikiApiHelper.GetApiForChannel(channel);
            return rc;
        }

        private void ConstructLogObject(RecentChange rc, string titleValue, string comment, string flagValue, string data, string channel)
        {
            rc.Log = titleValue.Split('/').Skip(1).First();
            rc.EditFlags = flagValue;

            bool handled = false;

            switch (rc.Log)
            {
                case "abusefilter":
                
                    if (rc.EditFlags == "hit")
                    {
                        var match = new Regex(" triggered \\[\\[(?<page>Special:AbuseFilter/(?<filter>[0-9]+))\\|filter \\k<filter>\\]\\], performing the action \"(?<action>.*?)\" on \\[\\[(?<targetpage>.*?)\\]\\]. Actions taken: (?<actions>.*?) \\(\\[\\[Special:AbuseLog/[0-9]+\\|details\\]\\]\\)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["targetpage"].Value;
                            rc.EditFlags += "; " + result.Groups["action"].Value;
                            rc.AdditionalData = result.Groups["actions"].Value;

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "create" || rc.EditFlags == "modify")
                    {
                        var match = new Regex(" (?:created|modified) \\[\\[(?<page>Special:AbuseFilter/[0-9]+)\\]\\] \\(\\[\\[Special:AbuseFilter/history/[0-9]+/diff/prev/[0-9]+\\]\\]\\)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }

                    break;
                case "block":
                    if (rc.EditFlags == "block")
                    {
                        var match = new Regex("^blocked (?:\\[\\[)?User:(?<targetUser>.*?)(?:\\]\\])?(?:[ ]*)(?:\\((?<flags>.*?)\\))? with an (?:expiry|expiration) time of (?<expiry>.*?)(?: \\((?<flags2>.*?)\\))?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            if (result.Groups["flags"].Success)
                            {
                                rc.EditFlags += ", " + result.Groups["flags"].Value;   
                            }

                            if (result.Groups["flags2"].Success)
                            {
                                rc.EditFlags += ", " + result.Groups["flags2"].Value;   
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "reblock")
                    {
                        var match = new Regex("^changed block settings for \\[\\[User:(?<targetUser>.*?)\\]\\](?:[ ]*)(?:\\((?<flags>.*?)\\))? with an (?:expiry|expiration) time of (?<expiry>.*?)(?: \\((?<flags2>.*?)\\))?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            if (result.Groups["flags"].Success)
                            {
                                rc.EditFlags += ", " + result.Groups["flags"].Value;
                            }

                            if (result.Groups["flags2"].Success)
                            {
                                rc.EditFlags += ", " + result.Groups["flags2"].Value;   
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "unblock")
                    {
                        var match = new Regex("^unblocked User:(?<targetUser>.*?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }

                    break;
                case "contentmodel":
                    if (rc.EditFlags == "change")
                    {
                        var match = new Regex(" changed the content model of the page \\[\\[(?<pageName>.*?)\\]\\] from \"(?<oldContentmodel>.*?)\" to \"(?<newContentmodel>.*?)\"(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "new")
                    {
                        var match = new Regex(" created the page \\[\\[(?<pageName>.*?)\\]\\] using a non-default content model \"(?<contentModel>.*?)\"(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }


                            handled = true;
                        }
                    }
                    
                    break;
                case "delete":
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex("^deleted \"\\[\\[(?<pageName>.*?)\\]\\]\"(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    if (rc.EditFlags == "restore")
                    {
                        var match = new Regex("^restored \"\\[\\[(?<pageName>.*?)\\]\\]\"(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "delete_redir" || rc.EditFlags == "delete_redir2")
                    {
                        var match = new Regex(@"deleted redirect \[\[(?<pageName>.*?)\]\] by overwriting(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "revision")
                    {
                        var match = new Regex(@"changed visibility of (?:[0-9]+ revisions|a revision) on page \[\[(?<pageName>.*?)\]\]: (?:[a-z, ]*)hidden(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "event")
                    {
                        var match = new Regex(@" changed visibility of (?:a log event|[0-9]+ log events) on \[\[(?<pageName>.*?)\]\]: (?:[a-z, ]*)hidden(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    break;
                case "gblblock":
                    if (rc.EditFlags == "whitelist")
                    {
                        var match = new Regex(@" disabled the global block on \[\[User:(?<targetUser>.*?)\]\] locally(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "dwhitelist")
                    {
                        var match = new Regex(@" re-enabled the global block on \[\[User:(?<targetUser>.*?)\]\] locally(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "gblock2")
                    {
                        var match = new Regex(@"^globally blocked \[\[User:(?<targetUser>.*?)\]\] \(.*?\)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "modify")
                    {
                        var match = new Regex(@"^modified the global block on \[\[User:(?<targetUser>.*?)\]\] \(.*?\)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "gunblock")
                    {
                        var match = new Regex(@"^removed global block on \[\[User:(?<targetUser>.*?)\]\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    break;
                case "gblrename":
                    if (rc.EditFlags == "rename")
                    {
                        var match = new Regex(@" globally renamed \[\[Special:CentralAuth/(?<targetUser>.*?)\]\] to \[\[Special:CentralAuth/(.*?)\]\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    break;
                case "gblrights":
                    if (rc.EditFlags == "usergroups")
                    {
                        var match = new Regex(@"changed global group membership for (?:\[\[)?User:(?<targetUser>.+?)(?:\]\])? from (?:.+?) to (?:.+?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "groupprms2")
                    {
                        var match = new Regex(@"^changed global group permissions for .*?: added .*?; removed .*?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "groupprms3")
                    {
                        var match = new Regex(@"^changed group restricted wikis set for Special:GlobalUsers/.*? from .*? to .*?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "setchange")
                    {
                        var match = new Regex(@"^changed wikis in ""(?<page>.*?)"": added: (?:.*?); removed: (?:.*?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }

                    if (rc.EditFlags == "grouprename")
                    {
                        var match = new Regex(@"^renamed group Special:GlobalGroupPermissions/(?:.*?) to Special:GlobalGroupPermissions/(?:.*?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                case "globalauth":
                    if (rc.EditFlags == "setstatus")
                    {
                        var match = new Regex(@"changed status for global account ""(?:\[\[)?User:(?<targetUser>.*?)@global(?:\]\])?"": set .*?; unset .*?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex(@"^deleted global account ""User:(?<targetUser>.*?)@global""(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["targetUser"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    break;
                case "import":
                    if (rc.EditFlags == "interwiki")
                    {
                        var match = new Regex(@"^transwikied (?<pageName>.*?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "upload")
                    {
                        var match = new Regex(@"^imported \[\[(?<pageName>.*?)\]\] by file upload(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                case "managetags":
                    if (rc.EditFlags == "create")
                    {
                        var match = new Regex(@" created the tag ""(?<pageName>.*?)""(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "deactivate")
                    {
                        var match = new Regex(@" deactivated the tag ""(?<pageName>.*?)"" for use by users and bots(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex(@" deleted the tag ""(?<pageName>.*?)"" \(removed from [0-9]+ revision(?:s?) (?:and/)?or log entr(?:ies|y)\)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["pageName"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                case "massmessage":
                    if (rc.EditFlags == "skipnouser")
                    {
                        var match = new Regex("^Delivery of \"(?<page>.*?)\" to \\[\\[User talk:(?<targetuser>.*?)\\]\\] was skipped because .*$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetUser = result.Groups["targetuser"].Value;

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "skipoptout")
                    {
                        var match = new Regex("^Delivery of \"(?<page>.*?)\" to \\[\\[(?<targetpage>.*?)\\]\\] was skipped because .*$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["targetpage"].Value;

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "send")
                    {
                        var match = new Regex(" sent page \\[\\[(?<page>.*?)\\]\\] as message to \\[\\[(?<target>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["target"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "failure")
                    {
                        var match = new Regex("^Delivery of \"(?<comment>.*?)\" to \\[\\[(?<page>.*?)\\]\\] failed with an error code of .*$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "skipbadns")
                    {
                        var match = new Regex("^Delivery of \"(?<comment>.*?)\" to \\[\\[(?<page>.*?)\\]\\] was skipped because target was in a namespace that cannot be posted in$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    break;
                case "merge":
                    if (rc.EditFlags == "merge")
                    {
                        var match = new Regex(@"^merged \[\[(?<oldPage>.*?)\]\] into \[\[(?<newPage>.*?)\]\] \(revisions up to [0-9]+\)(?:: Merged \[\[:\k<oldPage>\]\] into \[\[:\k<newPage>\]\])?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["oldPage"].Value;
                            rc.TargetPage = result.Groups["newPage"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                case "move":
                    if (rc.EditFlags == "move")
                    {
                        var match = new Regex("^moved \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<newpage>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    if (rc.EditFlags == "move_redir")
                    {
                        var match = new Regex("^moved \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<newpage>.*?)\\]\\] over redirect(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

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

                    if (rc.EditFlags == "create2" || rc.EditFlags == "byemail")
                    {
                        var match = new Regex("^created new account User:(?<targetuser>.*?)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            rc.TargetUser = result.Groups["targetuser"].Value;

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "forcecreatelocal")
                    {
                        var match = new Regex("forcibly created a local account for \\[\\[User:(?<targetuser>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            rc.TargetUser = result.Groups["targetuser"].Value;

                            handled = true;
                        }
                    }
                    break;
                case "notifytranslators":
                    if (rc.EditFlags == "sent")
                    {
                        var match = new Regex(" sent a notification about translating page \\[\\[(?<page>.*?)\\]\\]; .*");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
            
                            handled = true;
                        }
                    }
                    
                    break;
                case "pagelang":
                    if (rc.EditFlags == "pagelang")
                    {
                        var match = new Regex(" changed the language of \\[\\[(?<page>.*?)\\]\\] from .*$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
            
                            handled = true;
                        }
                    }

                    break;
                case "pagetranslation":
                    if (rc.EditFlags == "associate" || rc.EditFlags == "dissociate")
                    {
                        var match = new Regex(" (added|removed) translatable page \\[\\[(?<page>.*?)\\]\\] (to|from) aggregate group ");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "encourage")
                    {
                        var match = new Regex(" encouraged translation of \\[\\[(?<page>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "mark")
                    {
                        var match = new Regex(" marked \\[\\[(?<page>.*?)\\]\\] for translation$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "movenok")
                    {
                        var match = new Regex(" encountered a problem while moving page \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<targetpage>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["targetpage"].Value;
            
                            handled = true;
                        }
                    }
                    if (rc.EditFlags == "moveok")
                    {
                        var match = new Regex(" completed renaming of translatable page \\[\\[(?<page>.*?)\\]\\] to \\[\\[(?<targetpage>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["targetpage"].Value;
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "deletelok")
                    {
                        var match = new Regex(" completed deletion of translation page \\[\\[(?<page>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "deletelnok")
                    {
                        var match = new Regex(" failed to delete \\[\\[(?:.*?)\\]\\] which belongs to translation page \\[\\[(?<page>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "deletefok")
                    {
                        var match = new Regex(" completed deletion of translatable page \\[\\[(?<page>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "discourage")
                    {
                        var match = new Regex(" discouraged translation of \\[\\[(?<page>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "prioritylanguages")
                    {
                        var match = new Regex(" (?:removed priority languages from|set the priority languages for|limited languages for) translatable page \\[\\[(?<page>.*?)\\]\\](?: to .+?)?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "unmark")
                    {
                        var match = new Regex(" removed \\[\\[(?<page>.*?)\\]\\] from the translation system$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            handled = true;
                        }
                    }

                    break;
                case "pagetriage-copyvio":
                    if (rc.EditFlags == "insert")
                    {
                        var match = new Regex("marked revision (?<revision>[0-9]+) on \\[\\[(?<page>.*?)\\]\\] as a potential copyright violation$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;

                            handled = true;
                        }
                    }
                    break;
                case "pagetriage-curation":
                    if (rc.EditFlags == "reviewed")
                    {
                        var match = new Regex("marked \\[\\[(?<page>.*?)\\]\\] as reviewed(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "unreviewed")
                    {
                        var match = new Regex("marked \\[\\[(?<page>.*?)\\]\\] as unreviewed(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex("marked \\[\\[(?<page>.*?)\\]\\] for deletion");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "enqueue")
                    {
                        var match = new Regex(" added \\[\\[(?<page>.*?)\\]\\] to the New Pages Feed");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "tag")
                    {
                        var match = new Regex("tagged \\[\\[(?<page>.*?)\\]\\] with");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    break;
                case "pagetriage-deletion":
                    if (rc.EditFlags == "delete")
                    {
                        var match = new Regex("marked \\[\\[(?<page>.*?)\\]\\] for deletion");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "patrol":
                    if (rc.EditFlags == "patrol")
                    {
                        var match = new Regex("marked revision [0-9]+ of \\[\\[(?<page>.*)\\]\\] patrolled $");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "protect":
                    if (rc.EditFlags == "protect")
                    {
                        var match = new Regex(@"^protected ""\[\[(?<page>.*?) .(?:\[(?:edit|move|create|upload)=[a-z-]+\] \((?:(?:expires .*?\(UTC\))|indefinite)\).?)+\]\]""(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "modify")
                    {
                        var match = new Regex(@"^changed protection (?:settings|level) (?:of|for) (?:""\[\[)?(?<page>.*?) .(?:\[(?:edit|move|create|upload)=[a-z-]+\] \((?:(?:expires .*?\(UTC\))|indefinite)\).?)+(?:\]\]"")?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "unprotect")
                    {
                        var match = new Regex(@"^removed protection from ""\[\[(?<page>.*?)\]\]""(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            rc.Page = result.Groups["page"].Value;
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "move_prot")
                    {
                        var match = new Regex(@"^moved protection settings from ""\[\[(?<page>.*?)\]\]"" to ""\[\[(?<newpage>.*?)\]\]"": \[\[\k<page>\]\] moved to \[\[\k<newpage>\]\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                
                case "renameuser":
                    if (rc.EditFlags == "renameuser")
                    {
                        var match = new Regex(" renamed user \\[\\[User:(?<oldname>.*?)\\]\\] \\([0-9]+ edit(?:s?)\\) to \\[\\[User:(?<newname>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["oldname"].Value;
                            rc.AdditionalData = result.Groups["newname"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "review":
                    if (rc.EditFlags == "approve" || rc.EditFlags == "unapprove")
                    {
                        var match = new Regex(" (?:reviewed|deprecated) a version of \\[\\[(?<page>.*?)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "rights":
                    if (rc.EditFlags == "autopromote")
                    {
                        var match = new Regex("was automatically updated (?<changes>from [a-z-, ()]+ to [a-z-, ()]+)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.AdditionalData = result.Groups["changes"].Value;
                            
                            handled = true;
                        }
                    } 
                    if (rc.EditFlags == "blockautopromote")
                    {
                        var match = new Regex(" blocked the autopromotion of \\[\\[User:(?<user>.*)\\]\\] for a period of .*?: (?<comment>.*)$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["user"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }                            
                            handled = true;
                        }
                    }  
                    if (rc.EditFlags == "restoreautopromote")
                    {
                        var match = new Regex(" restored the autopromotion capability of \\[\\[User:(?<user>.*)\\]\\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["user"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }                            
                            handled = true;
                        }
                    }
                    if (rc.EditFlags == "rights")
                    {
                        var match = new Regex("changed group membership for User:(?<user>.*?) (?<changes>from [a-z-, ()]+ to [a-z-, ()]+)(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.TargetUser = result.Groups["user"].Value;
                            rc.AdditionalData = result.Groups["changes"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    break;
                
                case "stable":
                    if (rc.EditFlags == "config" || rc.EditFlags == "reset")
                    {
                        var match = new Regex(@" (?:configured|reset) pending changes settings for \[\[(?<page>.*?)\]\] (?:\[Auto-(?:accept|review): .*?\])?(?: ?)(?:\((?:(?:expires .*?\(UTC\))|indefinite)\))?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "move_stable")
                    {
                        var match = new Regex(@" moved pending changes settings from \[\[(?<page>.*?)\]\] to \[\[(?<newpage>.*?)\]\]: \[\[\k<page>\]\] moved to \[\[\k<newpage>\]\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            rc.TargetPage = result.Groups["newpage"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "modify")
                    {
                        var match = new Regex(@" altered pending changes settings for \[\[(?<page>.*?)\]\] (?:\[Auto-(?:accept|review): .*?\])?(?: ?)(?:\((?:(?:expires .*?\(UTC\))|indefinite)\))?(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;

                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }

                            handled = true;
                        }
                    }
                    
                    break;
                
                case "tag":
                    if (rc.EditFlags == "update")
                    {
                        var match = new Regex(@" (?:added|removed) the tag .*? (?:to|from) (?:log entry|revision) [0-9]+ of page \[\[(?<page>.*?)\]\](?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
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
                case "translationreview":
                    if (rc.EditFlags == "message")
                    {
                        var match = new Regex(" reviewed translation \\[\\[(?<page>.*?)\\]\\]$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            handled = true;
                        }
                    }
                    
                    if (rc.EditFlags == "group")
                    {
                        var match = new Regex(" changed the state of .*? translations of \\[\\[.*?\\|(?<page>.*?)\\]\\] from .*? to .*?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            handled = true;
                        }
                    }

                    break;
                case "upload":
                    if (rc.EditFlags == "overwrite" || rc.EditFlags == "upload" || rc.EditFlags == "revert")
                    {
                        var match = new Regex("uploaded(?: a new version of)? \"\\[\\[(?<page>.*?)\\]\\]\"(?:: (?<comment>.*))?$");
                        var result = match.Match(comment);
                        if (result.Success)
                        {
                            rc.Page = result.Groups["page"].Value;
                            
                            if (result.Groups["comment"].Success)
                            {
                                rc.EditSummary = result.Groups["comment"].Value;
                            }
                            
                            handled = true;
                        }
                    }
                    
                    break;
            }

            if (!handled)
            {
                throw new BugException(
                    string.Format("Unhandled log entry of type {0} / {1}", rc.Log, rc.EditFlags),
                    string.Format("```\n{0}\n```\n```\n{1}\n```\nFrom: {2}", comment, data, channel));
            }

            if (!string.IsNullOrWhiteSpace(rc.Page) && rc.Page.StartsWith("Special:AbuseFilter/") && string.IsNullOrWhiteSpace(rc.Url))
            {
                // Abusefilter hit
                var filterId = rc.Page.Split('/')[1];
                var timestamp = DateTime.UtcNow.ToString("O");

                rc.Url = "https://en.wikipedia.org/wiki/Special:AbuseLog?wpSearchPeriodEnd=" + timestamp + "&wpSearchFilter=" + filterId;
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