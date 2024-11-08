namespace EyeInTheSky.Tests.Services.RecentChanges.Irc
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges.Irc;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    [TestFixture]
    public class IrcRecentChangeParserTests : TestBase
    {
        private IrcRecentChangeParser rcparser;

        [SetUp]
        public void LocalSetup()
        {
            var helperMock = Substitute.For<IMediaWikiApiHelper>();
            helperMock.GetApiForChannel(Arg.Any<string>()).Returns((IMediaWikiApi)null);
            
            this.rcparser = new IrcRecentChangeParser(this.LoggerMock, helperMock);
        }

        [Test, TestCaseSource(typeof(IrcRecentChangeParserTests), nameof(ParseTestData))]
        public IRecentChange ShouldParseCorrectly(string data)
        {
            var shouldParseCorrectly = this.rcparser.Parse(data, "");
            this.LoggerMock.DidNotReceive().ErrorFormat(Arg.Any<string>(), Arg.Any<object[]>());
            return shouldParseCorrectly;
        }

        [Test, TestCaseSource(typeof(IrcRecentChangeParserTests), nameof(ParseLogTestData))]
        public IRecentChange ShouldParseLogCorrectly(string data)
        {
            var shouldParseLogCorrectly = this.rcparser.Parse(data, "");
            this.LoggerMock.DidNotReceive().ErrorFormat(Arg.Any<string>(), Arg.Any<object[]>());

            return shouldParseLogCorrectly;
        }

        [Test]
        public void ShouldErrorIfUnknownLogType()
        {
            // arrange
            string data = "14[[07Special:Log/fakelog14]]4 sdfsdf10 02 5* 03Jimbo 5*  10Loggy message";

            // act
            Assert.Throws<LogParseException>(() => this.rcparser.Parse(data, ""));
        }

        public static IEnumerable<TestCaseData> ParseTestData
        {
            get
            {
                yield return new TestCaseData(
                        "14[[07List of Sites of Special Scientific Interest in Kent14]]4 10 02https://en.wikipedia.org/w/index.php?diff=831707969&oldid=831707719 5* 03Dudley Miles 5* (+2) 10")
                    .Returns(
                        new RecentChange("Dudley Miles")
                        {
                            Page = "List of Sites of Special Scientific Interest in Kent",
                            Url = "https://en.wikipedia.org/w/index.php?diff=831707969&oldid=831707719",
                            SizeDiff = 2
                        });

                yield return new TestCaseData(
                        "14[[07Matthew Lodge14]]4 M10 02https://en.wikipedia.org/w/index.php?diff=831708179&oldid=831696958 5* 03Doctorhawkes 5* (+45) 10Reverted edits by [[Special:Contribs/122.108.247.131|122.108.247.131]] ([[User talk:122.108.247.131|talk]]) to last version by Fleets")
                    .Returns(
                        new RecentChange("Doctorhawkes")
                        {
                            Page = "Matthew Lodge",
                            Url = "https://en.wikipedia.org/w/index.php?diff=831708179&oldid=831696958",
                            EditSummary = "Reverted edits by [[Special:Contribs/122.108.247.131|122.108.247.131]] ([[User talk:122.108.247.131|talk]]) to last version by Fleets",
                            EditFlags = "M",
                            SizeDiff = 45
                        });

                yield return new TestCaseData(
                        "14[[07Myrmecia gratiosa14]]4 MB10 02https://en.wikipedia.org/w/index.php?diff=831707968&oldid=804753497 5* 03Tom.Bot 5* (+29) 10[[User:Tom.Bot/Task3|Task 3]]: +{{Taxonbar|[[:Category:Taxonbar templates without from parameter|from]]=[[d:Special:EntityPage/Q13868803|Q13868803]]}} ([[WT:TREE#Taxonbar addition requirements|2 sig. taxon IDs]]); WP:GenFix using [[Project:AWB|AWB]]")
                    .Returns(
                        new RecentChange("Tom.Bot")
                        {
                            Page = "Myrmecia gratiosa",
                            Url = "https://en.wikipedia.org/w/index.php?diff=831707968&oldid=804753497",
                            EditSummary = "[[User:Tom.Bot/Task3|Task 3]]: +{{Taxonbar|[[:Category:Taxonbar templates without from parameter|from]]=[[d:Special:EntityPage/Q13868803|Q13868803]]}} ([[WT:TREE#Taxonbar addition requirements|2 sig. taxon IDs]]); WP:GenFix using [[Project:AWB|AWB]]",
                            EditFlags = "MB",
                            SizeDiff = 29
                        });

                yield return new TestCaseData(
                        "14[[07User:Erodr017/be bold14]]4 !N10 02https://en.wikipedia.org/w/index.php?oldid=831708535&rcid=1038874003 5* 03Erodr017 5* (+38) 10automatic post as part of sandbox guided tour")
                    .Returns(
                        new RecentChange("Erodr017")
                        {
                            Page = "User:Erodr017/be bold",
                            Url = "https://en.wikipedia.org/w/index.php?oldid=831708535&rcid=1038874003",
                            EditSummary = "automatic post as part of sandbox guided tour",
                            EditFlags = "!N",
                            SizeDiff = 38
                        });
            }
        }

        public static IEnumerable<TestCaseData> ParseLogTestData
        {
            get
            {
                #region abusefilter
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 hit10 02 5* 03GriffinMorganHuff 5*  10GriffinMorganHuff triggered [[Special:AbuseFilter/550|filter 550]], performing the action \"edit\" on [[02The Boy in Blue (1986 film)10]]. Actions taken: Tag ([[Special:AbuseLog/20813489|details]])")
                    .Returns(
                        new RecentChange("GriffinMorganHuff")
                        {
                            Page = "Special:AbuseFilter/550",
                            TargetPage = "The Boy in Blue (1986 film)",
                            EditFlags = "hit; edit",
                            Log = "abusefilter",
                            AdditionalData = "Tag"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 hit10 02 5* 03Ceanneisenhammer 5*  10Ceanneisenhammer triggered [[Special:AbuseFilter/527|filter 527]], performing the action \"createaccount\" on [[02Special:UserLogin10]]. Actions taken: none ([[Special:AbuseLog/20724740|details]])")
                    .Returns(
                        new RecentChange("Ceanneisenhammer")
                        {
                            Page = "Special:AbuseFilter/527",
                            TargetPage = "Special:UserLogin",
                            EditFlags = "hit; createaccount",
                            Log = "abusefilter",
                            AdditionalData = "none"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 create10 02 5* 03Beetstra 5*  10Beetstra created [[02Special:AbuseFilter/91010]] ([[Special:AbuseFilter/history/910/diff/prev/18570]])")
                    .Returns(
                        new RecentChange("Beetstra")
                        {
                            Page = "Special:AbuseFilter/910",
                            EditFlags = "create",
                            Log = "abusefilter"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 modify10 02 5* 03Cyp 5*  10Cyp modified [[02Special:AbuseFilter/89810]] ([[Special:AbuseFilter/history/898/diff/prev/18571]])")
                    .Returns(
                        new RecentChange("Cyp")
                        {
                            Page = "Special:AbuseFilter/898",
                            EditFlags = "modify",
                            Log = "abusefilter"
                        });                
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 hit10 02 5* 03Awake4life 5*  10Awake4life {{GENDER:Awake4life|triggered}} [[Special:AbuseFilter/867|filter 867]], {{GENDER:Awake4life|performing}} the action \"edit\" on [[02Awake For Days10]]. Actions taken: none ([[Special:AbuseLog/34345850|details]])")
                    .Returns(
                        new RecentChange("Awake4life")
                        {
                            Page = "Special:AbuseFilter/898",
                            EditFlags = "modify",
                            Log = "abusefilter"
                        }).Ignore("Test is broken but prior ignore reason was resolved.");
                #endregion
                #region block
                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 block10 02 5* 03MSGJ 5*  10blocked User:142.196.0.207 (account creation blocked) with an expiry time of 31 hours: [[WP:Edit warring|Edit warring]]")
                    .Returns(
                        new RecentChange("MSGJ")
                        {
                            Log = "block",
                            TargetUser = "142.196.0.207",
                            Expiry = TimeSpan.FromHours(31),
                            EditSummary = "[[WP:Edit warring|Edit warring]]",
                            EditFlags = "block, account creation blocked"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 block10 02 5* 03DESiegel 5*  10blocked User:Downtowndistrict  with an expiry time of indefinite: {{uw-ublock}}")
                    .Returns(
                        new RecentChange("DESiegel")
                        {
                            Log = "block",
                            TargetUser = "Downtowndistrict",
                            EditSummary = "{{uw-ublock}}",
                            EditFlags = "block"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 reblock10 02 5* 03SQL 5*  10changed block settings for [[02User:188.29.165.18910]] (anon. only, account creation blocked, cannot edit own talk page) with an expiry time of 11:18, June 29, 2018: see also 86.28.94.117")
                    .Returns(
                        new RecentChange("SQL")
                        {
                            Log = "block",
                            TargetUser = "188.29.165.189",
                            Expiry = TimeSpan.FromHours(31),
                            EditSummary = "see also 86.28.94.117",
                            EditFlags = "reblock, anon. only, account creation blocked, cannot edit own talk page"
                        });     
                
                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 reblock10 02 5* 03Sir Sputnik 5*  10changed block settings for [[02User:Albertpda10]]  with an expiry time of indefinite: Abusing [[WP:SOCK|multiple accounts]]: Please see: [[Wikipedia:Sockpuppet investigations/Albertpda]]")
                    .Returns(
                        new RecentChange("Sir Sputnik")
                        {
                            Log = "block",
                            TargetUser = "Albertpda",
                            Expiry = null,
                            EditSummary = "Abusing [[WP:SOCK|multiple accounts]]: Please see: [[Wikipedia:Sockpuppet investigations/Albertpda]]",
                            EditFlags = "reblock"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 unblock10 02 5* 03PhilKnight 5*  10unblocked User:NHBuchanan: following request")
                    .Returns(
                        new RecentChange("PhilKnight")
                        {
                            Log = "block",
                            TargetUser = "NHBuchanan",
                            EditSummary = "following request",
                            EditFlags = "unblock"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 block10 02 5* 03RadiX 5*  10blocked [[02User:181.15.157.216/2910]] with an expiration time of 6 months (account creation disabled, cannot edit own talk page): Cross-wiki spam: leaky webhost; should you be affected by this block, please [[:m:Special:Contact/Stewards|contact us]]")
                    .Returns(
                        new RecentChange("RadiX")
                        {
                            Log = "block",
                            TargetUser = "181.15.157.216/29",
                            EditSummary = "Cross-wiki spam: leaky webhost; should you be affected by this block, please [[:m:Special:Contact/Stewards|contact us]]",
                            EditFlags = "block, account creation disabled, cannot edit own talk page"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 block10 02 5* 03Defender 5*  10blocked [[02User:Other LT-'AS Copy Gl'apz10]] with an expiration time of infinite (account creation disabled, email disabled, cannot edit own talk page)")
                    .Returns(
                        new RecentChange("Defender")
                        {
                            Log = "block",
                            TargetUser = "Other LT-'AS Copy Gl'apz",
                            EditFlags = "block, account creation disabled, email disabled, cannot edit own talk page"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 reblock10 02 5* 03Billinghurst 5*  10changed block settings for [[02User:194.187.219.14610]] with an expiration time of 3 months (anonymous users only, account creation disabled, email disabled, cannot edit own talk page): [[w:Open proxy|Open proxy]] or [[w:Zombie computer|zombie]] ([[No open proxies|more info]]): [[WM:VN|Vandalism]]")
                    .Returns(
                        new RecentChange("Billinghurst")
                        {
                            Log = "block",
                            TargetUser = "194.187.219.146",
                            EditFlags = "reblock, anonymous users only, account creation disabled, email disabled, cannot edit own talk page",
                            Expiry = new TimeSpan(90,0,0,0),
                            EditSummary = "[[w:Open proxy|Open proxy]] or [[w:Zombie computer|zombie]] ([[No open proxies|more info]]): [[WM:VN|Vandalism]]"
                        });
                
                
                #endregion
                #region contentmodel
                //faked
                yield return new TestCaseData(
                        "14[[07Special:Log/contentmodel14]]4 change10 02 5* 03TheDJ 5*  10TheDJ changed the content model of the page [[Wikipedia:WikiProject User scripts/Scripts/Get user name]] from \"wikitext\" to \"JavaScript\"")
                    .Returns(
                        new RecentChange("TheDJ")
                        {
                            Log = "contentmodel",
                            Page = "Wikipedia:WikiProject User scripts/Scripts/Get user name",
                            EditFlags = "change"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/contentmodel14]]4 change10 02 5* 03Isarra 5*  10Isarra changed the content model of the page [[02User:Isarra/test10]] from \"JSON\" to \"plain text\": Is this even possible?")
                    .Returns(
                        new RecentChange("Isarra")
                        {
                            Log = "contentmodel",
                            Page = "User:Isarra/test",
                            EditFlags = "change",
                            EditSummary = "Is this even possible?"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/contentmodel14]]4 new10 02 5* 03Xaosflux 5*  10Xaosflux created the page [[02Wikipedia:WikiProject Christianity/Outreach/Full content delivery/210]] using a non-default content model \"MassMessageListContent\": Create mass message delivery list")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "contentmodel",
                            Page = "Wikipedia:WikiProject Christianity/Outreach/Full content delivery/2",
                            EditFlags = "new",
                            EditSummary = "Create mass message delivery list"
                        });
                #endregion
                #region delete
                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 revision10 02 5* 03Hut 8.5 5*  10Hut 8.5 changed visibility of 5 revisions on page [[02Humza Yousaf10]]: content hidden: [[WP:RD1|RD1]]: Copyright violations: https://inews.co.uk/news/politics/humza-yousaf-scottish-transport-minister-facing-5000-fine-driving-uninsured/")
                    .Returns(
                        new RecentChange("Hut 8.5")
                        {
                            Log = "delete",
                            Page = "Humza Yousaf",
                            EditFlags = "revision",
                            EditSummary = "[[WP:RD1|RD1]]: Copyright violations: https://inews.co.uk/news/politics/humza-yousaf-scottish-transport-minister-facing-5000-fine-driving-uninsured/"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 revision10 02 5* 03Mz7 5*  10Mz7 changed visibility of a revision on page [[02Footlights10]]: edit summary hidden and content unhidden: [[WP:RD2|RD2]]: Serious [[WP:BLP|BLP]] violations: violation is in the edit summary")
                    .Returns(
                        new RecentChange("Mz7")
                        {
                            Log = "delete",
                            Page = "Footlights",
                            EditFlags = "revision",
                            EditSummary = "[[WP:RD2|RD2]]: Serious [[WP:BLP|BLP]] violations: violation is in the edit summary"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 delete10 02 5* 03Maile66 5*  10deleted \"[[02Talk:How to possible 50 Mbps speed in Just Rs.5000 per Year10]]\": [[WP:CSD#G8|G8]]: Page dependent on a deleted or nonexistent page")
                    .Returns(
                        new RecentChange("Maile66")
                        {
                            Log = "delete",
                            EditFlags = "delete",
                            EditSummary = "[[WP:CSD#G8|G8]]: Page dependent on a deleted or nonexistent page",
                            Page = "Talk:How to possible 50 Mbps speed in Just Rs.5000 per Year"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 delete_redir10 02 5* 03WikiOriginal-9 5*  10WikiOriginal-9 deleted redirect [[02David Carr (American football)10]] by overwriting: [[WP:CSD#G6|G6]]: Deleted to make way for move")
                    .Returns(
                        new RecentChange("WikiOriginal-9")
                        {
                            Log = "delete",
                            Page = "David Carr (American football)",
                            EditSummary = "[[WP:CSD#G6|G6]]: Deleted to make way for move",
                            EditFlags = "delete_redir"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 delete_redir210 02 5* 03Polyamorph 5*  10Polyamorph deleted redirect [[02Tiptree Heath10]] by overwriting: [[WP:CSD#G6|G6]]: Deleted to make way for move")
                    .Returns(
                        new RecentChange("Polyamorph")
                        {
                            Log = "delete",
                            Page = "Tiptree Heath",
                            EditSummary = "[[WP:CSD#G6|G6]]: Deleted to make way for move",
                            EditFlags = "delete_redir2"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 restore10 02 5* 03Graeme Bartlett 5*  10restored \"[[02Draft:June Beverly Bonesteel10]]\": requested by FlyladyAZ")
                    .Returns(
                        new RecentChange("Graeme Bartlett")
                        {
                            Log = "delete",
                            Page = "Draft:June Beverly Bonesteel",
                            EditSummary = "requested by FlyladyAZ",
                            EditFlags = "restore"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 restore10 02 5* 03Anthony Appleyard 5*  10restored \"[[02Talk:Hakea exul10]]\"")
                    .Returns(
                        new RecentChange("Anthony Appleyard")
                        {
                            Log = "delete",
                            Page = "Talk:Hakea exul",
                            EditFlags = "restore"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 restore10 02 5* 03Less Unless 5*  10undeleted \"[[02Draft talk:John Bankett10]]\": Undeleting [[Help:Talk page|talk page]] of \"Draft:John Bankett\" ([[User:SD0001/RFUD-helper|rfud-helper]])")
                    .Returns(
                        new RecentChange("Less Unless")
                        {
                            Log = "delete",
                            Page = "Draft talk:John Bankett",
                            EditFlags = "restore",
                            EditSummary = "Undeleting [[Help:Talk page|talk page]] of \"Draft:John Bankett\" ([[User:SD0001/RFUD-helper|rfud-helper]])"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 event10 02 5* 03Oshwah 5*  10Oshwah changed visibility of a log event on [[02Special:Log/block10]]: content hidden and username hidden: [[WP:RD3|RD3]]: Purely disruptive material")
                    .Returns(
                        new RecentChange("Oshwah")
                        {
                            Log = "delete",
                            Page = "Special:Log/block",
                            EditFlags = "event",
                            EditSummary = "[[WP:RD3|RD3]]: Purely disruptive material"
                        });
                
                yield return new TestCaseData(
                    "14[[07Special:Log/delete14]]4 event10 02 5* 03Od Mishehu 5*  10Od Mishehu changed visibility of 2 log events on [[02Special:Log10]]: content hidden: [[WP:RD3|RD3]]: Purely disruptive material: user name")
                    .Returns(
                        new RecentChange("Od Mishehu")
                        {
                            Log = "delete",
                            Page = "Special:Log",
                            EditFlags = "event",
                            EditSummary = "[[WP:RD3|RD3]]: Purely disruptive material: user name"
                        });
                #endregion
                #region gblblock
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 whitelist10 02 5* 03Zzuuzz 5*  10Zzuuzz disabled the global block on [[02User:85.255.236.0/2310]] locally: too much collateral; contact me if there's any issues")
                    .Returns(
                        new RecentChange("Zzuuzz")
                        {
                            Log = "gblblock",
                            TargetUser = "85.255.236.0/23",
                            EditFlags = "whitelist",
                            EditSummary = "too much collateral; contact me if there's any issues"
                        });
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/gblblock\u000314]]\u00034 whitelist\u000310 \u000302\u0003 \u00035*\u0003 \u000303Xaosflux\u0003 \u00035*\u0003  \u000310Xaosflux disabled the global block on Xaosflux ep locally: test\u0003")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "gblblock",
                            TargetUser = "Xaosflux ep",
                            EditFlags = "whitelist",
                            EditSummary = "test"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 dwhitelist10 02 5* 03Galobtter 5*  10Galobtter re-enabled the global block on [[02User:138.197.0.0/1610]] locally: Subranges have all been blocked")
                    .Returns(
                        new RecentChange("Galobtter")
                        {
                            Log = "gblblock",
                            TargetUser = "138.197.0.0/16",
                            EditFlags = "dwhitelist",
                            EditSummary = "Subranges have all been blocked"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 gblock210 02 5* 03RadiX 5*  10globally blocked [[02User:84.244.90.0/2410]] (expiration 02:41, 10 April 2019): Cross-wiki spam")
                    .Returns(
                        new RecentChange("RadiX")
                        {
                            Log = "gblblock",
                            TargetUser = "84.244.90.0/24",
                            EditFlags = "gblock2",
                            EditSummary = "Cross-wiki spam"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 gblock210 02 5* 03Tegel 5*  10globally blocked [[02User:2605:3E80:D00:10:0:0:0:0/6410]] (anonymous only, expiration 16:44, 27 January 2019): Long-term abuse")
                    .Returns(
                        new RecentChange("Tegel")
                        {
                            Log = "gblblock",
                            TargetUser = "2605:3E80:D00:10:0:0:0:0/64",
                            EditFlags = "gblock2",
                            EditSummary = "Long-term abuse"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 modify10 02 5* 03RadiX 5*  10modified the global block on [[02User:172.255.125.13710]] (expiration 03:35, 25 January 2020): [[m:NOP|Open proxy]]")
                    .Returns(
                        new RecentChange("RadiX")
                        {
                            Log = "gblblock",
                            TargetUser = "172.255.125.137",
                            EditFlags = "modify",
                            EditSummary = "[[m:NOP|Open proxy]]"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/gblblock\u000314]]\u00034 modify\u000310 \u000302\u0003 \u00035*\u0003 \u000303JJMC89\u0003 \u00035*\u0003  \u000310JJMC89 changed global block settings for 184.94.215.0/24 with an expiration time of 01:40, 22 April 2027 : [[m:Special:MyLanguage/NOP|Open proxy/Webhost]]: See the [[m:WM:OP/H|help page]] if you are affected: <!-- NameCheap -->\u0003")
                    .Returns(
                        new RecentChange("JJMC89")
                        {
                            Log = "gblblock",
                            TargetUser = "184.94.215.0/24",
                            EditFlags = "modify",
                            EditSummary = "[[m:Special:MyLanguage/NOP|Open proxy/Webhost]]: See the [[m:WM:OP/H|help page]] if you are affected: <!-- NameCheap -->"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 gunblock10 02 5* 03Rxy 5*  10removed global block on [[02User:2001:2C0:DB07:3E00:0:0:0:0/6410]]: done")
                    .Returns(
                        new RecentChange("Rxy")
                        {
                            Log = "gblblock",
                            TargetUser = "2001:2C0:DB07:3E00:0:0:0:0/64",
                            EditFlags = "gunblock",
                            EditSummary = "done"
                        });                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 gunblock10 02 5* 03Teles 5*  10Teles removed global block on [[02User:151.236.23.13610]]: Blocked for too long")
                    .Returns(
                        new RecentChange("Teles")
                        {
                            Log = "gblblock",
                            TargetUser = "151.236.23.136",
                            EditFlags = "gunblock",
                            EditSummary = "Blocked for too long"
                        });
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/gblblock\u000314]]\u00034 gunblock\u000310 \u000302\u0003 \u00035*\u0003 \u000303Yahya\u0003 \u00035*\u0003  \u000310Yahya removed the global block on 202.235.158.57: range blocked\u0003")
                    .Returns(
                        new RecentChange("Yahya")
                        {
                            Log = "gblblock",
                            TargetUser = "202.235.158.57",
                            EditFlags = "gunblock",
                            EditSummary = "range blocked"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/gblblock\u000314]]\u00034 gblock2\u000310 \u000302\u0003 \u00035*\u0003 \u000303Tegel\u0003 \u00035*\u0003  \u000310Tegel globally blocked [[\u000302User:205.213.208.200\u000310]] (expiration 14:23, 1 May 2023): Cross-wiki vandalism\u0003")
                    .Returns(
                        new RecentChange("Tegel")
                        {
                            Log = "gblblock",
                            TargetUser = "205.213.208.200",
                            EditFlags = "gblock2",
                            EditSummary = "Cross-wiki vandalism"
                        });
                yield return new TestCaseData(
                        "14[[07Special:Log/gblblock14]]4 modify10 02 5* 03Sotiale 5*  10Sotiale modified the global block on [[02User:37.161.131.21210]] (anonymous only, expiration 15:23, 30 January 2023): Cross-wiki vandalism")
                    .Returns(
                        new RecentChange("Sotiale")
                        {
                            Log = "gblblock",
                            TargetUser = "37.161.131.212",
                            EditFlags = "modify",
                            EditSummary = "Cross-wiki vandalism"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/gblblock\u000314]]\u00034 gblock\u000310 \u000302\u0003 \u00035*\u0003 \u000303Tegel\u0003 \u00035*\u0003  \u000310Tegel globally blocked 212.97.66.17 with an expiration time of 22:11, 20 March 2025 : [[m:Special:MyLanguage/NOP|Open proxy]]: See the [[m:WM:OP/H|help page]] if you are affected\u0003")
                    .Returns(
                        new RecentChange("Tegel")
                        {
                            Log = "gblblock",
                            TargetUser = "212.97.66.17",
                            EditFlags = "gblock",
                            EditSummary = "[[m:Special:MyLanguage/NOP|Open proxy]]: See the [[m:WM:OP/H|help page]] if you are affected"
                        });
                #endregion
                #region gblrename

                yield return new TestCaseData(
                        "14[[07Special:Log/gblrename14]]4 rename10 02 5* 03Bencemac 5*  10Bencemac globally renamed [[Special:CentralAuth/Roland Kiadó]] to [[02Special:CentralAuth/Kékelefánt10]]: Per [[w:hu:Special:Permalink/20868144#Roland Kiadó .E2.86.92 Kékelefánt|request on huwiki]]")
                    .Returns(
                        new RecentChange("Bencemac")
                        {
                            Log = "gblrename",
                            TargetUser = "Roland Kiadó",
                            EditFlags = "rename",
                            EditSummary = "Per [[w:hu:Special:Permalink/20868144#Roland Kiadó .E2.86.92 Kékelefánt|request on huwiki]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrename14]]4 promote10 02 5* 03Litlok 5*  10Litlok globally renamed [[User:Саша Попов@metawiki]] to [[02Special:CentralAuth/Саша Попов Александр Алексеевич10]]: per [[m:Special:GlobalRenameQueue/request/92371|request]]")
                    .Returns(
                        new RecentChange("Litlok")
                        {
                            Log = "gblrename",
                            TargetUser = "Саша Попов",
                            EditFlags = "promote",
                            EditSummary = "per [[m:Special:GlobalRenameQueue/request/92371|request]]"
                        });
                
                #endregion
                #region gblrights

                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 usergroups10 02 5* 03MarcoAurelio 5*  10changed global group membership for User:StraussInTheHouse from (none) to oathauth-tester: [[Special:Permalink/18775617]]")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "gblrights",
                            TargetUser = "StraussInTheHouse",
                            EditFlags = "usergroups",
                            EditSummary = "[[Special:Permalink/18775617]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 usergroups10 02 5* 03Trijnstel 5*  10Trijnstel changed global group membership for [[02User:Hamish10]] from global-ipblock-exempt, global-rollbacker, vrt-permissions to global-ipblock-exempt, global-rollbacker: per [[m:Special:Permalink/22429296|request]]")
                    .Returns(
                        new RecentChange("Trijnstel")
                        {
                            Log = "gblrights",
                            TargetUser = "Hamish",
                            EditFlags = "usergroups",
                            EditSummary = "per [[m:Special:Permalink/22429296|request]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 usergroups10 02 5* 03Martin Urbanec 5*  10Martin Urbanec changed global group membership for Gregology from (none) to oathauth-tester: per [[m:Special:Permalink/22798808|request]]")
                    .Returns(
                        new RecentChange("Martin Urbanec")
                        {
                            Log = "gblrights",
                            TargetUser = "Gregology",
                            EditFlags = "usergroups",
                            EditSummary = "per [[m:Special:Permalink/22798808|request]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 usergroups10 02 5* 03Tks4Fish 5*  10Tks4Fish changed global group membership for Rhodesian05 from (none) to global-ipblock-exempt (temporary, until 17:35, 9 May 2022): per [[Special:Permalink/22799756#IP locking|request]]")
                    .Returns(
                        new RecentChange("Tks4Fish")
                        {
                            Log = "gblrights",
                            TargetUser = "Rhodesian05",
                            EditFlags = "usergroups",
                            EditSummary = "per [[Special:Permalink/22799756#IP locking|request]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 usergroups10 02 5* 03Martin Urbanec 5*  10Martin Urbanec changed global group membership for NguoiDungKhongDinhDanh from oathauth-tester to oathauth-tester, global-rollbacker: per [[m:Special:Permalink/22802474|request]]: clear consensus to promote")
                    .Returns(
                        new RecentChange("Martin Urbanec")
                        {
                            Log = "gblrights",
                            TargetUser = "NguoiDungKhongDinhDanh",
                            EditFlags = "usergroups",
                            EditSummary = "per [[m:Special:Permalink/22802474|request]]: clear consensus to promote"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 setchange10 02 5* 03MF-Warburg 5*  10changed wikis in \"All existing wikis\": added: liwikinews, satwiki, shnwiki, yuewiktionary, zhwikiversity; removed: (–): update")
                    .Returns(
                        new RecentChange("MF-Warburg")
                        {
                            Log = "gblrights",
                            Page = "All existing wikis",
                            EditFlags = "setchange",
                            EditSummary = "update"
                        });          
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 setchange10 02 5* 03MarcoAurelio 5*  10changed wikis in \"Global bot wikis\": added: kywikibooks; removed: (none): [[b:ky:Special:Permalink/4616#Bot_policy|no objections]]")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "gblrights",
                            Page = "Global bot wikis",
                            EditFlags = "setchange",
                            EditSummary = "[[b:ky:Special:Permalink/4616#Bot_policy|no objections]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 setchange10 02 5* 03Sotiale 5*  10Sotiale changed wikis in \"All existing wikis\": added: lmowiktionary, amiwiki, pwnwiki; removed: (none): 2021 created wikis update")
                    .Returns(
                        new RecentChange("Sotiale")
                        {
                            Log = "gblrights",
                            Page = "All existing wikis",
                            EditFlags = "setchange",
                            EditSummary = "2021 created wikis update"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 groupprms210 02 5* 03Rxy 5*  10changed global group permissions for Special:GlobalUsers/steward: added (none); removed bigdelete: -bigdelete: dangerous things; no need this usually")
                    .Returns(
                        new RecentChange("Rxy")
                        {
                            Log = "gblrights",
                            EditFlags = "groupprms2",
                            EditSummary = "-bigdelete: dangerous things; no need this usually"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 groupprms210 02 5* 03Majavah 5*  10Majavah changed global group permissions for [[02Special:GlobalUsers/sysadmin10]]: added centralauth-suppress; removed (none): +centralauth-suppress, already granted centralauth-oversight being renamed in [[phab:T302675]]")
                    .Returns(
                        new RecentChange("Majavah")
                        {
                            Log = "gblrights",
                            EditFlags = "groupprms2",
                            EditSummary = "+centralauth-suppress, already granted centralauth-oversight being renamed in [[phab:T302675]]"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 groupprms310 02 5* 03Tks4Fish 5*  10changed group restricted wikis set for Special:GlobalUsers/abusefilter-maintainer from (none) to All existing wikis: Creating per community consensus at [[:m:Special:Permalink/19949707]]")
                    .Returns(
                        new RecentChange("Tks4Fish")
                        {
                            Log = "gblrights",
                            EditFlags = "groupprms3",
                            EditSummary = "Creating per community consensus at [[:m:Special:Permalink/19949707]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 groupprms310 02 5* 03Ruslik0 5*  10Ruslik0 changed group restricted wikis set for [[02Special:GlobalUsers/global-sysop10]] from Opted-out of global sysop wikis to (none): per a discussion at SRM")
                    .Returns(
                        new RecentChange("Ruslik0")
                        {
                            Log = "gblrights",
                            EditFlags = "groupprms3",
                            EditSummary = "per a discussion at SRM"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 grouprename10 02 5* 03Tks4Fish 5*  10renamed group Special:GlobalGroupPermissions/Abuse filter maintainer to Special:GlobalGroupPermissions/abusefilter-maintainer: Creating per community consensus at [[:m:Special:Permalink/19949707]]")
                    .Returns(
                        new RecentChange("Tks4Fish")
                        {
                            Log = "gblrights",
                            EditFlags = "grouprename",
                            EditSummary = "Creating per community consensus at [[:m:Special:Permalink/19949707]]"
                        });
                yield return new TestCaseData(
                        "14[[07Special:Log/gblrights14]]4 grouprename10 02 5* 03Vermont 5*  10Vermont renamed group [[Special:GlobalGroupPermissions/staff]] to [[Special:GlobalGroupPermissions/wmf-supportsafety]]")
                    .Returns(
                        new RecentChange("Vermont")
                        {
                            Log = "gblrights",
                            EditFlags = "grouprename",
                        });

                #endregion
                #region globalauth
                yield return new TestCaseData(
                        "14[[07Special:Log/globalauth14]]4 setstatus10 02 5* 03Rxy 5*  10changed status for global account \"User:テイルズオブ田中綾子@global\": set locked; unset (none): Long-term abuse: [[w:ja:LTA:ISECHIKA]] or copycat<!--automatic-->")
                    .Returns(
                        new RecentChange("Rxy")
                        {
                            Log = "globalauth",
                            TargetUser = "テイルズオブ田中綾子",
                            EditFlags = "setstatus",
                            EditSummary = "Long-term abuse: [[w:ja:LTA:ISECHIKA]] or copycat<!--automatic-->"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/globalauth14]]4 delete10 02 5* 03Tegel 5*  10deleted global account \"User:Plaster93@global\"")
                    .Returns(
                        new RecentChange("Tegel")
                        {
                            Log = "globalauth",
                            TargetUser = "Plaster93",
                            EditFlags = "delete",
                        });
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/globalauth\u000314]]\u00034 delete\u000310 \u000302\u0003 \u00035*\u0003 \u000303Martin Urbanec (WMF)\u0003 \u00035*\u0003  \u000310Martin Urbanec (WMF) deleted global account \"[[\u000302User:U4C@global\u000310]]\": testing again with Superpes\u0003")
                    .Returns(
                        new RecentChange("Martin Urbanec (WMF)")
                        {
                            Log = "globalauth",
                            TargetUser = "U4C",
                            EditFlags = "delete",
                            EditSummary = "testing again with Superpes"
                        });
                yield return new TestCaseData(
                        "14[[07Special:Log/globalauth14]]4 setstatus10 02 5* 03Masti 5*  10Masti changed status for global account \"[[02User:Theda420192666@global10]]\": set locked; unset (none): Spam-only account: spambot")
                    .Returns(
                        new RecentChange("Masti")
                        {
                            Log = "globalauth",
                            TargetUser = "Theda420192666",
                            EditFlags = "setstatus",
                            EditSummary = "Spam-only account: spambot"
                        });
                #endregion
                #region growthexperiments
                
                yield return new TestCaseData("14[[07Special:Log/growthexperiments14]]4 claimmentee10 02 5* 03Justiyaya 5*  10Justiyaya claimed Justiyaya as their mentee (previous mentor Bilorv): Claiming myself")
                    .Returns(
                        new RecentChange("Justiyaya")
                        {
                            Log = "growthexperiments",
                            TargetUser = "Justiyaya",
                            AlternateTargetUser = "Bilorv",
                            EditSummary = "Claiming myself",
                            EditFlags = "claimmentee"
                        });
                
                yield return new TestCaseData("14[[07Special:Log/growthexperiments14]]4 claimmentee10 02 5* 03Blaze The Wolf 5*  10Blaze The Wolf claimed KingAviationKid as their mentee (previous mentor Vukky): They have requested that I be my mentor after asking their previous mentor if I can be their mentor instead.")
                    .Returns(
                        new RecentChange("Blaze The Wolf")
                        {
                            Log = "growthexperiments",
                            TargetUser = "KingAviationKid",
                            AlternateTargetUser = "Vukky",
                            EditSummary = "They have requested that I be my mentor after asking their previous mentor if I can be their mentor instead.",
                            EditFlags = "claimmentee"
                        });
                
                yield return new TestCaseData("14[[07Special:Log/growthexperiments14]]4 claimmentee-no-previous-mentor10 02 5* 03Vukky 5*  10Vukky claimed Syskerr91 as their mentee (no previous mentor): peekaboo! i know you, so you're mine now")
                    .Returns(
                        new RecentChange("Vukky")
                        {
                            Log = "growthexperiments",
                            TargetUser = "Syskerr91",
                            EditSummary = "peekaboo! i know you, so you're mine now",
                            EditFlags = "claimmentee-no-previous-mentor"
                        });
                
                yield return new TestCaseData("14[[07Special:Log/growthexperiments14]]4 setmentor10 02 5* 03Xaosflux 5*  10Xaosflux set Usedtobecool as the mentor of ShahidK1980 (previous mentor Xaosflux): ReleaseAutoAssignedMenteeToPool")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "growthexperiments",
                            TargetUser = "Usedtobecool",
                            AlternateTargetUser = "ShahidK1980",
                            EditSummary = "ReleaseAutoAssignedMenteeToPool",
                            EditFlags = "setmentor"
                        });
                
                yield return new TestCaseData("\u000314[[\u000307Special:Log/growthexperiments\u000314]]\u00034 addimage\u000310 \u000302\u0003 \u00035*\u0003 \u000303Android-Test17-WMF\u0003 \u00035*\u0003  \u000310Android-Test17-WMF reviewed an image suggestion for [[\u000302July 1974\u000310]]: rejected\u0003")
                    .Returns(
                        new RecentChange("Android-Test17-WMF")
                        {
                            Log = "growthexperiments",
                            Page = "July 1974",
                            EditSummary = "rejected",
                            EditFlags = "addimage"
                        });
                #endregion
                #region import
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/import14]]4 interwiki10 02 5* 03Graham87 5*  10transwikied Tasmanian tiger: import old edit, see [[User:Graham87/Import]]")
                    .Returns(
                        new RecentChange("Graham87")
                        {
                            Log = "import",
                            Page = "Tasmanian tiger",
                            EditSummary = "import old edit, see [[User:Graham87/Import]]",
                            EditFlags = "interwiki"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/import14]]4 upload10 02 5* 03Graham87 5*  10imported [[02Aachen10]] by file upload: import old edit from [[nost:Aachen]], see [[User:Graham87/Import]]")
                    .Returns(
                        new RecentChange("Graham87")
                        {
                            Log = "import",
                            Page = "Aachen",
                            EditSummary = "import old edit from [[nost:Aachen]], see [[User:Graham87/Import]]",
                            EditFlags = "upload"
                        });
                
                #endregion
                #region managetags
                //faked
                yield return new TestCaseData(
                        "14[[07Special:Log/managetags14]]4 create10 02 5* 03West.andrew.g 5*  10West.andrew.g created the tag \"STiki\": Tracking edits made by the [[WP:STiki]] tool")
                    .Returns(
                        new RecentChange("West.andrew.g")
                        {
                            Log = "managetags",
                            Page = "STiki",
                            EditFlags = "create",
                            EditSummary = "Tracking edits made by the [[WP:STiki]] tool"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/managetags14]]4 deactivate10 02 5* 03AGK 5*  10AGK deactivated the tag \"PAWS\" for use by users and bots: misclicked")
                    .Returns(
                        new RecentChange("AGK")
                        {
                            Log = "managetags",
                            Page = "PAWS",
                            EditFlags = "deactivate",
                            EditSummary = "misclicked"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/managetags\u000314]]\u00034 activate\u000310 \u000302\u0003 \u00035*\u0003 \u000303EggRoll97\u0003 \u00035*\u0003  \u000310EggRoll97 activated the tag \"uncategorized-redirect\" for use by users and bots: Reactivate\u0003")
                    .Returns(
                        new RecentChange("EggRoll97")
                        {
                            Log = "managetags",
                            Page = "uncategorized-redirect",
                            EditFlags = "activate",
                            EditSummary = "Reactivate"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/managetags14]]4 delete10 02 5* 03MusikAnimal 5*  10MusikAnimal deleted the tag \"long-term-abuse\" (removed from 15 revisions and/or log entries): Per [[Special:Permalink/864524082#Please review 937]], may do more harm than good")
                    .Returns(
                        new RecentChange("MusikAnimal")
                        {
                            Log = "managetags",
                            Page = "long-term-abuse",
                            EditFlags = "delete",
                            EditSummary = "Per [[Special:Permalink/864524082#Please review 937]], may do more harm than good"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/managetags14]]4 delete10 02 5* 03Premeditated Chaos 5*  10Premeditated Chaos deleted the tag \"predatory\" (removed from 1 revision or log entry): created in error")
                    .Returns(
                        new RecentChange("Premeditated Chaos")
                        {
                            Log = "managetags",
                            Page = "predatory",
                            EditFlags = "delete",
                            EditSummary = "created in error"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/managetags\u000314]]\u00034 delete\u000310 \u000302\u0003 \u00035*\u0003 \u000303XXBlackburnXx\u0003 \u00035*\u0003  \u000310XXBlackburnXx deleted the tag \"New article without categories, templates\" (removed from 1,405 revisions and/or log entries): typo\u0003")
                    .Returns(
                        new RecentChange("XXBlackburnXx")
                        {
                            Log = "managetags",
                            Page = "New article without categories, templates",
                            EditFlags = "delete",
                            EditSummary = "typo"
                        });
                
                #endregion
                #region massmessage
                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 skipnouser10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"Project Tiger Writing Contest\" to [[02User talk:TiruTiruTiru10]] was skipped because the user account does not exist")
                    .Returns(
                        new RecentChange("MediaWiki message delivery")
                        {
                            Log = "massmessage",
                            Page = "Project Tiger Writing Contest",
                            TargetUser = "TiruTiruTiru",
                            EditFlags = "skipnouser"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 skipoptout10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"This Month in Education: March 2018\" to [[02User talk:Dcoetzee10]] was skipped because the target has opted-out of message delivery")
                    .Returns(
                        new RecentChange("MediaWiki message delivery")
                        {
                            Log = "massmessage",
                            Page = "This Month in Education: March 2018",
                            TargetPage = "User talk:Dcoetzee",
                            EditFlags = "skipoptout"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 skipoptout10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"New Affiliations Committee appointments\" to [[02Talk:Commons Photographers User Group10]] was skipped because the target has opted-out of message delivery")
                    .Returns(
                        new RecentChange("MediaWiki message delivery")
                        {
                            Log = "massmessage",
                            Page = "New Affiliations Committee appointments",
                            TargetPage = "Talk:Commons Photographers User Group",
                            EditFlags = "skipoptout"
                        });

                //faked
                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 send10 02 5* 03Another Believer 5*  10Another Believer sent a message to [[Special:PermanentLink/830385617]]: Art+Feminism Wikipedia Edit-a-thon (April 13, University of Oregon)")
                    .Returns(
                        new RecentChange("Another Believer")
                        {
                            Log = "massmessage",
                            TargetPage = "Special:PermanentLink/830385617",
                            EditSummary = "Art+Feminism Wikipedia Edit-a-thon (April 13, University of Oregon)",
                            EditFlags = "send"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 send10 02 5* 03Quiddity (WMF) 5*  10Quiddity (WMF) sent page [[Tech/News/2021/48]] as message to [[Special:PermanentLink/19181792]]: [[m:Special:MyLanguage/Tech/News/2021/48|Tech News: 2021-48]]")
                    .Returns(
                        new RecentChange("Quiddity (WMF)")
                        {
                            Log = "massmessage",
                            Page = "Tech/News/2021/48",
                            TargetPage = "Special:PermanentLink/19181792",
                            EditSummary = "[[m:Special:MyLanguage/Tech/News/2021/48|Tech News: 2021-48]]",
                            EditFlags = "send"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 failure10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"Ichthus April 2018\" to [[02User talk:Sam Korn10]] failed with an error code of <code>protectedpage</code>")
                    .Returns(
                        new RecentChange("MediaWiki message delivery")
                        {
                            Log = "massmessage",
                            Page = "User talk:Sam Korn",
                            EditSummary = "Ichthus April 2018",
                            EditFlags = "failure"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 skipbadns10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"Wikipedia translation of the week: 2018-52\" to [[02Sadenar4000010]] was skipped because target was in a namespace that cannot be posted in")
                    .Returns(
                        new RecentChange("MediaWiki message delivery")
                        {
                            Log = "massmessage",
                            Page = "Sadenar40000",
                            EditSummary = "Wikipedia translation of the week: 2018-52",
                            EditFlags = "skipbadns"
                        });
                #endregion
                #region merge
                //faked
                yield return new TestCaseData(
                        "14[[07Special:Log/merge14]]4 merge10 02 5* 03West.andrew.g 5*  10merged [[User:Webmaster at Kentucky Today/sandbox]] into [[User:DESiegel/Kentucky Today]] (revisions up to 20180405191938): Merged [[:User:Webmaster at Kentucky Today/sandbox]] into [[:User:DESiegel/Kentucky Today]]: restore history for proper attribution and copyright")
                    .Returns(
                        new RecentChange("West.andrew.g")
                        {
                            Log = "merge",
                            Page = "User:Webmaster at Kentucky Today/sandbox",
                            TargetPage = "User:DESiegel/Kentucky Today",
                            EditFlags = "merge",
                            EditSummary = "restore history for proper attribution and copyright"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/merge14]]4 merge10 02 5* 03Oshwah 5*  10merged [[02Draft:Bandringa10]] into [[Bandringa]] (revisions up to 20180405074950): [[WP:HISTMERGE]] - Fixing cut and paste page move.")
                    .Returns(
                        new RecentChange("Oshwah")
                        {
                            Log = "merge",
                            Page = "Draft:Bandringa",
                            TargetPage = "Bandringa",
                            EditFlags = "merge",
                            EditSummary = "[[WP:HISTMERGE]] - Fixing cut and paste page move."
                        });
                #endregion
                #region move
                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move10 02 5* 03JaJaWa 5*  10moved [[02Yuanling Station10]] to [[Yuanling station]]")
                    .Returns(
                        new RecentChange("JaJaWa")
                        {
                            Log = "move",
                            Page = "Yuanling Station",
                            TargetPage = "Yuanling station",
                            EditFlags = "move"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move10 02 5* 03SamgeeGamwise 5*  10moved [[02Convents burning10]] to [[Convent burning]]: moving page due to incorrect pluralisation in title")
                    .Returns(
                        new RecentChange("SamgeeGamwise")
                        {
                            Log = "move",
                            Page = "Convents burning",
                            TargetPage = "Convent burning",
                            EditFlags = "move",
                            EditSummary = "moving page due to incorrect pluralisation in title"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move_redir10 02 5* 03WikiOriginal-9 5*  10moved [[02David Carr (quarterback)10]] to [[David Carr (American football)]] over redirect: no other American football player named David Carr")
                    .Returns(
                        new RecentChange("WikiOriginal-9")
                        {
                            Log = "move",
                            EditFlags = "move_redir",
                            Page = "David Carr (quarterback)",
                            TargetPage = "David Carr (American football)",
                            EditSummary = "no other American football player named David Carr"
                        });

                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move_redir10 02 5* 03sdfsdf 5*  10moved [[02David Carr (quarterback)10]] to [[David Carr (American football)]] over redirect")
                    .Returns(
                        new RecentChange("sdfsdf")
                        {
                            Log = "move",
                            EditFlags = "move_redir",
                            Page = "David Carr (quarterback)",
                            TargetPage = "David Carr (American football)"
                        });
                #endregion
                #region newusers
                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 create10 02 5* 03PiliPili Alex 5*  10New user account")
                    .Returns(
                        new RecentChange("PiliPili Alex")
                        {
                            Log = "newusers",
                            TargetUser = "PiliPili Alex",
                            EditFlags = "create"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 create210 02 5* 03Esthenia 5*  10created new account User:EstheniaOG")
                    .Returns(
                        new RecentChange("Esthenia")
                        {
                            Log = "newusers",
                            TargetUser = "EstheniaOG",
                            EditFlags = "create2"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 byemail10 02 5* 03Ryder5656 5*  10created new account User:Ryder9997")
                    .Returns(
                        new RecentChange("Ryder5656")
                        {
                            Log = "newusers",
                            TargetUser = "Ryder9997",
                            EditFlags = "byemail"
                        });

                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 byemail10 02 5* 03Ryder5656 5*  10created new account User:Ryder9997: Requested at ACC")
                    .Returns(
                        new RecentChange("Ryder5656")
                        {
                            Log = "newusers",
                            TargetUser = "Ryder9997",
                            EditFlags = "byemail",
                            EditSummary = "Requested at ACC",
                            
                        });
                
                
                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 forcecreatelocal10 02 5* 03GeneralNotability 5*  10GeneralNotability forcibly created a local account for [[02User:Yzf9910]]: Forcing attach, Ticket#2021122410002159")
                    .Returns(
                        new RecentChange("GeneralNotability")
                        {
                            Log = "newusers",
                            TargetUser = "Yzf99",
                            EditFlags = "forcecreatelocal",
                            EditSummary = "Forcing attach, Ticket#2021122410002159",
                        });
                
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/newusers\u000314]]\u00034 forcecreatelocal\u000310 \u000302\u0003 \u00035*\u0003 \u000303Xaosflux\u0003 \u00035*\u0003  \u000310Xaosflux forcibly created a local account for Bottomlessfries: merge to SUL\u0003")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "newusers",
                            TargetUser = "Bottomlessfries",
                            EditFlags = "forcecreatelocal",
                            EditSummary = "merge to SUL",
                        });
                
                #endregion
                #region notifytranslators
                yield return new TestCaseData(
                        "14[[07Special:Log/notifytranslators14]]4 sent10 02 5* 03MarcoAurelio 5*  10MarcoAurelio sent a notification about translating page [[02Admin activity review/Notice to inactive right holders10]]; languages: de, de-formal; deadline: none; priority: medium; sent to 171 recipients, failed for 0 recipients, skipped for 0 recipients")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "notifytranslators",
                            Page = "Admin activity review/Notice to inactive right holders",
                            EditFlags = "sent"
                        });
                #endregion
                #region pagelang
                yield return new TestCaseData(
                        "14[[07Special:Log/pagelang14]]4 pagelang10 02 5* 03Benoit Rochon 5*  10Benoit Rochon changed the language of [[02WikiFranca/Plan d'action10]] from English (en) [default] to French (fr)")
                    .Returns(
                        new RecentChange("Benoit Rochon")
                        {
                            Log = "pagelang",
                            Page = "WikiFranca/Plan d'action",
                            EditFlags = "pagelang"
                        });
                #endregion
                #region pagetranslation
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 associate10 02 5* 03Jon Harald Søby (WMNO) 5*  10Jon Harald Søby (WMNO) added translatable page [[02Template:Uralic contest 201910]] to aggregate group Uralic language contest 2019")
                    .Returns(
                        new RecentChange("Jon Harald Søby (WMNO)")
                        {
                            Log = "pagetranslation",
                            Page = "Template:Uralic contest 2019",
                            EditFlags = "associate"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 mark10 02 5* 03Rodelar 5*  10Rodelar marked [[02The Wikipedia Library/1Lib1Ref10]] for translation")
                    .Returns(
                        new RecentChange("Rodelar")
                        {
                            Log = "pagetranslation",
                            Page = "The Wikipedia Library/1Lib1Ref",
                            EditFlags = "mark"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 moveok10 02 5* 03AVasanth (WMF) 5*  10AVasanth (WMF) completed renaming of translatable page [[02The Wikipedia Library/1Lib1Ref/Portal10]] to [[Template:The Wikipedia Library 1lib1ref navigation portal]]")
                    .Returns(
                        new RecentChange("AVasanth (WMF)")
                        {
                            Log = "pagetranslation",
                            Page = "The Wikipedia Library/1Lib1Ref/Portal",
                            TargetPage = "Template:The Wikipedia Library 1lib1ref navigation portal",
                            EditFlags = "moveok"
                        });
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/pagetranslation\u000314]]\u00034 moveok\u000310 \u000302\u0003 \u00035*\u0003 \u000303Minorax\u0003 \u00035*\u0003  \u000310Minorax completed renaming of translatable page [[\u000302Template:Former staff WMIT/text/en\u000310]] to [[Template:Former staff WMIT/text]]: fix\u0003")
                    .Returns(
                        new RecentChange("Minorax")
                        {
                            Log = "pagetranslation",
                            Page = "Template:Former staff WMIT/text/en",
                            TargetPage = "Template:Former staff WMIT/text",
                            EditFlags = "moveok",
                            EditSummary = "fix"
                        });
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 movenok10 02 5* 03MCruz (WMF) 5*  10MCruz (WMF) encountered a problem while moving page [[02Community Engagement Insights/2018 Report/Community Resources10]] to [[Community Insights/2018 Report/Community Resources]]")
                    .Returns(
                        new RecentChange("MCruz (WMF)")
                        {
                            Log = "pagetranslation",
                            Page = "Community Engagement Insights/2018 Report/Community Resources",
                            TargetPage = "Community Insights/2018 Report/Community Resources",
                            EditFlags = "movenok"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 deletelok10 02 5* 03MarcoAurelio 5*  10MarcoAurelio completed deletion of translation page [[02Neutral point of view/gsw10]]: vandalism")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "pagetranslation",
                            Page = "Neutral point of view/gsw",
                            EditFlags = "deletelok",
                            EditSummary = "vandalism"
                        }); 
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 deletelnok10 02 5* 03MarcoAurelio 5*  10MarcoAurelio failed to delete [[02Translations:Template:User oversighter/translatable/1/ro10]] which belongs to translation page [[Template:User oversighter/translatable/ro]]: banned contributor")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "pagetranslation",
                            Page = "Template:User oversighter/translatable/ro",
                            EditFlags = "deletelnok",
                            EditSummary = "banned contributor"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 deletefok10 02 5* 03Xaosflux 5*  10Xaosflux completed deletion of translatable page [[02Template:1lib1ref navigation portal10]]: cleanup following move")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "pagetranslation",
                            Page = "Template:1lib1ref navigation portal",
                            EditFlags = "deletefok",
                            EditSummary = "cleanup following move"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 deletefnok10 02 5* 03MusikAnimal (WMF) 5*  10MusikAnimal (WMF) failed to delete [[02Community Wishlist Survey 2022/Admins and patrollers/Foobar/Proposal/he10]] which belongs to translatable page [[Community Wishlist Survey 2022/Admins and patrollers/Foobar/Proposal]]: test complete")
                    .Returns(
                        new RecentChange("MusikAnimal (WMF)")
                        {
                            Log = "pagetranslation",
                            Page = "Community Wishlist Survey 2022/Admins and patrollers/Foobar/Proposal",
                            TargetPage = "Community Wishlist Survey 2022/Admins and patrollers/Foobar/Proposal/he",
                            EditFlags = "deletefnok",
                            EditSummary = "test complete"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 discourage10 02 5* 03GVarnum-WMF 5*  10GVarnum-WMF discouraged translation of [[02Wikimedia Foundation Chief Executive Officer and Executive Director/May 2016 office hours/Announcement10]]")
                    .Returns(
                        new RecentChange("GVarnum-WMF")
                        {
                            Log = "pagetranslation",
                            Page = "Wikimedia Foundation Chief Executive Officer and Executive Director/May 2016 office hours/Announcement",
                            EditFlags = "discourage"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 dissociate10 02 5* 03Chrumps 5*  10Chrumps removed translatable page [[02Special:Translate/page-Community health initiative/Per-user page, namespace, and upload blocking10]] from aggregate group Community Health")
                    .Returns(
                        new RecentChange("Chrumps")
                        {
                            Log = "pagetranslation",
                            Page = "Special:Translate/page-Community health initiative/Per-user page, namespace, and upload blocking",
                            EditFlags = "dissociate"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 prioritylanguages10 02 5* 03Quiddity (WMF) 5*  10Quiddity (WMF) limited languages for translatable page [[02User:ELappen (WMF)/Sandbox10]] to Arabic, French, German, Hindi, Japanese, Chinese, Portuguese, Russian, Spanish and Persian: per team request")
                    .Returns(
                        new RecentChange("Quiddity (WMF)")
                        {
                            Log = "pagetranslation",
                            Page = "User:ELappen (WMF)/Sandbox",
                            EditFlags = "prioritylanguages",
                            EditSummary = "per team request"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 prioritylanguages10 02 5* 03B1mbo 5*  10B1mbo set the priority languages for translatable page [[02Iberocoop:Iberoconf 2019/Carta de Santiago10]] to Spanish, English, Portuguese and Italian")
                    .Returns(
                        new RecentChange("B1mbo")
                        {
                            Log = "pagetranslation",
                            Page = "Iberocoop:Iberoconf 2019/Carta de Santiago",
                            EditFlags = "prioritylanguages"
                        });
                
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 prioritylanguages10 02 5* 03Elitre (WMF) 5*  10Elitre (WMF) removed priority languages from translatable page [[02Affiliate-selected Board seats/2019/Primer for user groups10]]")
                    .Returns(
                        new RecentChange("Elitre (WMF)")
                        {
                            Log = "pagetranslation",
                            Page = "Affiliate-selected Board seats/2019/Primer for user groups",
                            EditFlags = "prioritylanguages"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 prioritylanguages10 02 5* 03DannyS712 5*  10DannyS712 set the priority languages for translatable page [[02Cooperation of Wikimedia's Italian regional projects10]] to Italian, Piedmontese, Lombard, Ligurian, Venetian, Friulian, Emiliano-Romagnolo, Neapolitan, Tarantino, Sicilian, Sardinian, Corsican, Romagnol, Emilian, Sassarese Sardinian, French and German: Update version for translation")
                    .Returns(
                        new RecentChange("DannyS712")
                        {
                            Log = "pagetranslation",
                            Page = "Cooperation of Wikimedia's Italian regional projects",
                            EditFlags = "prioritylanguages",
                            EditSummary = "Update version for translation" 
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 prioritylanguages10 02 5* 03DannyS712 5*  10DannyS712 set the priority languages for translatable page [[02Wikimedia Sverige10]] to Swedish, Norwegian Bokmål, Portuguese and Russian: Update version for translation")
                    .Returns(
                        new RecentChange("DannyS712")
                        {
                            Log = "pagetranslation",
                            Page = "Wikimedia Sverige",
                            EditFlags = "prioritylanguages",
                            EditSummary = "Update version for translation" 
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 encourage10 02 5* 03DannyS712 5*  10DannyS712 encouraged translation of [[02Bridges across Cultures10]]")
                    .Returns(
                        new RecentChange("DannyS712")
                        {
                            Log = "pagetranslation",
                            Page = "Bridges across Cultures",
                            EditFlags = "encourage",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetranslation14]]4 unmark10 02 5* 03MarcoAurelio 5*  10MarcoAurelio removed [[02Meta:Sandbox10]] from the translation system")
                    .Returns(
                        new RecentChange("MarcoAurelio")
                        {
                            Log = "pagetranslation",
                            Page = "Meta:Sandbox",
                            EditFlags = "unmark"
                        });
                #endregion
                #region pagetriage
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-copyvio14]]4 insert10 02 5* 03EranBot 5*  10EranBot marked revision 914142196 on [[02Draft:IQOQI-Vienna10]] as a potential copyright violation")
                    .Returns(
                        new RecentChange("EranBot")
                        {
                            Log = "pagetriage-copyvio",
                            Page = "Draft:IQOQI-Vienna",
                            EditFlags = "insert"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 reviewed10 02 5* 03Natureium 5*  10Natureium marked [[02John Thomas (Republican advertising)10]] as reviewed")
                    .Returns(
                        new RecentChange("Natureium")
                        {
                            Log = "pagetriage-curation",
                            Page = "John Thomas (Republican advertising)",
                            EditFlags = "reviewed"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 reviewed10 02 5* 03Cwmhiraeth 5*  10Cwmhiraeth marked [[02Franco-German University10]] as reviewed: A well-written article and a useful addition to Wikipedia.")
                    .Returns(
                        new RecentChange("Cwmhiraeth")
                        {
                            Log = "pagetriage-curation",
                            Page = "Franco-German University",
                            EditFlags = "reviewed",
                            EditSummary = "A well-written article and a useful addition to Wikipedia."
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 unreviewed10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Veronica Cool10]] as unreviewed")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-curation",
                            Page = "Veronica Cool",
                            EditFlags = "unreviewed"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 unreviewed10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Carlos Roberto Signorelli10]] as unreviewed: The article as it is contains some content, but in no way passes review")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-curation",
                            Page = "Carlos Roberto Signorelli",
                            EditFlags = "unreviewed",
                            EditSummary = "The article as it is contains some content, but in no way passes review"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 delete10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Ak husky10]] for deletion with db-band tag")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-curation",
                            Page = "Ak husky",
                            EditFlags = "delete"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 tag10 02 5* 03Anaxial 5*  10Anaxial tagged [[02Farley, Staffordshire10]] with stub, uncategorised and unreferenced tags")
                    .Returns(
                        new RecentChange("Anaxial")
                        {
                            Log = "pagetriage-curation",
                            Page = "Farley, Staffordshire",
                            EditFlags = "tag"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 enqueue10 02 5* 03DannyS712 5*  10DannyS712 added [[02User:DannyS71210]] to the New Pages Feed")
                    .Returns(
                        new RecentChange("DannyS712")
                        {
                            Log = "pagetriage-curation",
                            Page = "User:DannyS712",
                            EditFlags = "enqueue"
                        });

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-deletion14]]4 delete10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Ak husky10]] for deletion with db-band tag")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-deletion",
                            Page = "Ak husky",
                            EditFlags = "delete"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/pagetriage-curation\u000314]]\u00034 reviewed-redirect\u000310 \u000302\u0003 \u00035*\u0003 \u000303DannyS712 bot III\u0003 \u00035*\u0003  \u000310DannyS712 bot III marked the redirect [[\u000302Marcos Loyalist Rallies\u000310]] as reviewed\u0003")
                    .Returns(
                        new RecentChange("DannyS712 bot III")
                        {
                            Log = "pagetriage-curation",
                            Page = "Marcos Loyalist Rallies",
                            EditFlags = "reviewed-redirect"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/pagetriage-curation\u000314]]\u00034 reviewed-article\u000310 \u000302\u0003 \u00035*\u0003 \u000303Hey man im josh\u0003 \u00035*\u0003  \u000310Hey man im josh marked the article [[\u000302Battle of Kota Batu (1578)\u000310]] as reviewed\u0003")
                    .Returns(
                        new RecentChange("Hey man im josh")
                        {
                            Log = "pagetriage-curation",
                            Page = "Battle of Kota Batu (1578)",
                            EditFlags = "reviewed-article"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/pagetriage-curation\u000314]]\u00034 unreviewed-article\u000310 \u000302\u0003 \u00035*\u0003 \u000303Timtrent\u0003 \u00035*\u0003  \u000310Timtrent marked the article [[\u000302J. Stanley Brown\u000310]] as unreviewed\u0003")
                    .Returns(
                        new RecentChange("Timtrent")
                        {
                            Log = "pagetriage-curation",
                            Page = "J. Stanley Brown",
                            EditFlags = "unreviewed-article"
                        });

                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/pagetriage-curation\u000314]]\u00034 unreviewed-redirect\u000310 \u000302\u0003 \u00035*\u0003 \u000303Illusion Flame\u0003 \u00035*\u0003  \u000310Illusion Flame marked the redirect [[\u000302Abdoul Abdouraguimov\u000310]] as unreviewed\u0003")
                    .Returns(
                        new RecentChange("Illusion Flame")
                        {
                            Log = "pagetriage-curation",
                            Page = "Abdoul Abdouraguimov",
                            EditFlags = "unreviewed-redirect"
                        });
                #endregion
                #region patrol
                yield return new TestCaseData(
                        "14[[07Special:Log/patrol14]]4 patrol10 02 5* 03SshibumXZ 5*  10marked revision 833886654 of [[02Sekyiwa Shakur10]] patrolled ")
                    .Returns(
                        new RecentChange("SshibumXZ")
                        {
                            Log = "patrol",
                            Page = "Sekyiwa Shakur",
                            EditFlags = "patrol"
                        });
                #endregion
                #region protect
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 protect10 02 5* 03Ohnoitsjamie 5*  10protected \"[[Alison Brie ‎[edit=autoconfirmed] (expires 20:56, 2 October 2018 (UTC))‎[move=autoconfirmed] (expires 20:56, 2 October 2018 (UTC))]]\": Persistent [[WP:Vandalism|vandalism]]")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Alison Brie",
                            EditFlags = "protect",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 protect10 02 5* 03Bongwarrior 5*  10protected \"[[20th Century Fox ‎[edit=autoconfirmed] (indefinite)‎[move=autoconfirmed] (indefinite)]]\": Persistent [[WP:Vandalism|vandalism]]")
                    .Returns(
                        new RecentChange("Bongwarrior")
                        {
                            Log = "protect",
                            Page = "20th Century Fox",
                            EditFlags = "protect",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 protect10 02 5* 03RHaworth 5*  10protected \"[[File:Moazzam Jah Ansari, IGP of Balochistan.jpg ‎[create=sysop] (indefinite)‎[upload=sysop] (indefinite)]]\": [[WP:SALT|Repeatedly recreated]]")
                    .Returns(
                        new RecentChange("RHaworth")
                        {
                            Log = "protect",
                            Page = "File:Moazzam Jah Ansari, IGP of Balochistan.jpg",
                            EditFlags = "protect",
                            EditSummary = "[[WP:SALT|Repeatedly recreated]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 move_prot10 02 5* 03Neutrality 5*  10moved protection settings from \"[[2018 YouTube headquarters shooting]]\" to \"[[02YouTube headquarters shooting10]]\": [[2018 YouTube headquarters shooting]] moved to [[02YouTube headquarters shooting10]]: Date is not necessary for disambiguation ")
                    .Returns(
                        new RecentChange("Neutrality")
                        {
                            Log = "protect",
                            Page = "2018 YouTube headquarters shooting",
                            TargetPage = "YouTube headquarters shooting",
                            EditFlags = "move_prot",
                            EditSummary = "Date is not necessary for disambiguation "
                        });
                
                // used by enwiki through message override
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 modify10 02 5* 03Ohnoitsjamie 5*  10changed protection level of Tom Kenny filmography ‎[edit=autoconfirmed] (expires 20:59, 2 October 2018 (UTC))‎[move=autoconfirmed] (expires 20:59, 2 October 2018 (UTC)): Persistent [[WP:Vandalism|vandalism]]: IP hopping crap")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Tom Kenny filmography",
                            EditFlags = "modify",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]: IP hopping crap"
                        });
                    
                // default MW message
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 modify10 02 5* 03Martin Urbanec 5*  10changed protection settings for \"[[MediaWiki:Centralnotice-stewnoms 2022-text2/sv ‎[edit=banner-protect] (indefinite)‎[move=banner-protect] (indefinite)]]\": Automatically protected by CentralNotice; please edit only via [[Special:CentralNotice]] or [[Special:Translate]]")
                    .Returns(
                        new RecentChange("Martin Urbanec")
                        {
                            Log = "protect",
                            Page = "MediaWiki:Centralnotice-stewnoms 2022-text2/sv",
                            EditFlags = "modify",
                            EditSummary = "Automatically protected by CentralNotice; please edit only via [[Special:CentralNotice]] or [[Special:Translate]]"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 modify10 02 5* 03Ohnoitsjamie 5*  10changed protection level of Tom Kenny filmography ‎[edit=autoconfirmed] (indefinite)‎[move=autoconfirmed] (expires 20:59, 2 October 2018 (UTC)): Persistent [[WP:Vandalism|vandalism]]: IP hopping crap")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Tom Kenny filmography",
                            EditFlags = "modify",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]: IP hopping crap"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 modify10 02 5* 03Ajraddatz 5*  10changed protection level for \"[[Talk:Steward requests/Global ‎[edit=autoconfirmed] (expires 17:41, 25 January 2019 (UTC))‎[move=sysop] (indefinite)]]\": Persistent vandalism")
                    .Returns(
                        new RecentChange("Ajraddatz")
                        {
                            Log = "protect",
                            Page = "Talk:Steward requests/Global",
                            EditFlags = "modify",
                            EditSummary = "Persistent vandalism"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 unprotect10 02 5* 03Ohnoitsjamie 5*  10removed protection from \"[[Keymon Ache]]\": Deprotecting to allow different draft to be accepted")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Keymon Ache",
                            EditFlags = "unprotect",
                            EditSummary = "Deprotecting to allow different draft to be accepted"
                        });
                #endregion
                #region renameuser
                yield return new TestCaseData(
                        "14[[07Special:Log/renameuser14]]4 renameuser10 02 5* 03Céréales Killer 5*  10Céréales Killer renamed user [[02User:SujaiRamPrasathC10]] (0 edits) to [[User:ZszasdojcqSsadaS]]: per [[m:Special:GlobalRenameQueue/request/41529|request]]")
                    .Returns(
                        new RecentChange("Céréales Killer")
                        {
                            Log = "renameuser",
                            TargetUser = "SujaiRamPrasathC",
                            AdditionalData = "ZszasdojcqSsadaS",
                            EditFlags = "renameuser",
                            EditSummary = "per [[m:Special:GlobalRenameQueue/request/41529|request]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/renameuser14]]4 renameuser10 02 5* 03Taketa 5*  10Taketa renamed user [[02User:Light2310]] (1 edit) to [[User:Herr.el]]: Per [[:w:en:Special:Permalink/833977985|en:WP:CHUS]]")
                    .Returns(
                        new RecentChange("Taketa")
                        {
                            Log = "renameuser",
                            TargetUser = "Light23",
                            AdditionalData = "Herr.el",
                            EditFlags = "renameuser",
                            EditSummary = "Per [[:w:en:Special:Permalink/833977985|en:WP:CHUS]]"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/renameuser\u000314]]\u00034 renameuser\u000310 \u000302\u0003 \u00035*\u0003 \u000303Nadzik\u0003 \u00035*\u0003  \u000310Nadzik renamed user [[\u000302User:Minimac\u000310]] (27,169 edits) to [[User:Abminor]]: per [[m:Special:GlobalRenameQueue/request/125198|request]]\u0003")
                    .Returns(
                        new RecentChange("Nadzik")
                        {
                            Log = "renameuser",
                            TargetUser = "Minimac",
                            AdditionalData = "Abminor",
                            EditFlags = "renameuser",
                            EditSummary = "per [[m:Special:GlobalRenameQueue/request/125198|request]]"
                        });
                #endregion
                #region review
                yield return new TestCaseData(
                        "14[[07Special:Log/review14]]4 approve10 02 5* 03GB fan 5*  10GB fan reviewed a version of [[02Electronic harassment10]]: ([[WP:TW|TW]])")
                    .Returns(
                        new RecentChange("GB fan")
                        {
                            Log = "review",
                            Page = "Electronic harassment",
                            EditFlags = "approve",
                            EditSummary = "([[WP:TW|TW]])"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/review14]]4 unapprove10 02 5* 03Zyc1174 5*  10Zyc1174 deprecated a version of [[02April 310]]")
                    .Returns(
                        new RecentChange("Zyc1174")
                        {
                            Log = "review",
                            Page = "April 3",
                            EditFlags = "unapprove"
                        });
                #endregion
                #region rights
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 blockautopromote10 02 5* 03Abuse filter 5*  10Abuse filter blocked the autopromotion of [[02User:Swagmaster4206942010]] for a period of 5 days: Autopromotion automatically delayed by abuse filter. Rule description: Blanking user pages or user talk pages by a new user")
                    .Returns(
                        new RecentChange("Abuse filter")
                        {
                            Log = "rights",
                            TargetUser = "Swagmaster42069420",
                            EditFlags = "blockautopromote",
                            EditSummary = "Autopromotion automatically delayed by abuse filter. Rule description: Blanking user pages or user talk pages by a new user",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 blockautopromote10 02 5* 03Abuse filter 5*  10Abuse filter blocked the autopromotion of Mr.Smith666 for a period of 5 days: Autopromotion automatically delayed by abuse filter. Rule description: Blanking user pages or user talk pages by a new user")
                    .Returns(
                        new RecentChange("Abuse filter")
                        {
                            Log = "rights",
                            TargetUser = "Mr.Smith666",
                            EditFlags = "blockautopromote",
                            EditSummary = "Autopromotion automatically delayed by abuse filter. Rule description: Blanking user pages or user talk pages by a new user",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 restoreautopromote10 02 5* 03Suffusion of Yellow 5*  10Suffusion of Yellow restored the autopromotion capability of [[02User:COIBot10]]: Autopromotion restored via AbuseFilter tools.")
                    .Returns(
                        new RecentChange("Suffusion of Yellow")
                        {
                            Log = "rights",
                            TargetUser = "COIBot",
                            EditFlags = "restoreautopromote",
                            EditSummary = "Autopromotion restored via AbuseFilter tools.",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 restoreautopromote10 02 5* 03Hasley 5*  10Hasley restored the autopromotion capability of PorkchopGMX: Autopromotion restored via AbuseFilter tools.")
                    .Returns(
                        new RecentChange("Hasley")
                        {
                            Log = "rights",
                            TargetUser = "PorkchopGMX",
                            EditFlags = "restoreautopromote",
                            EditSummary = "Autopromotion restored via AbuseFilter tools.",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 autopromote10 02 5* 03Popcrate 5*  10was automatically updated from (none) to extendedconfirmed")
                    .Returns(
                        new RecentChange("Popcrate")
                        {
                            Log = "rights",
                            EditFlags = "autopromote",
                            AdditionalData = "from (none) to extendedconfirmed"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/rights\u000314]]\u00034 autopromote\u000310 \u000302\u0003 \u00035*\u0003 \u000303Aafi\u0003 \u00035*\u0003  \u000310was automatically promoted from global-renamer, patroller, translationadmin to global-renamer, patroller, translationadmin, checkuser-temporary-account-viewer\u0003")
                    .Returns(
                        new RecentChange("Aafi")
                        {
                            Log = "rights",
                            EditFlags = "autopromote",
                            AdditionalData = "from global-renamer, patroller, translationadmin to global-renamer, patroller, translationadmin, checkuser-temporary-account-viewer"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/rights14]]4 rights10 02 5* 03CambridgeBayWeather 5*  10changed group membership for User:Outriggr from autoreviewer, extendedconfirmed, reviewer to autoreviewer, extendedconfirmed, reviewer, templateeditor: Needed")
                    .Returns(
                        new RecentChange("CambridgeBayWeather")
                        {
                            Log = "rights",
                            TargetUser = "Outriggr",
                            EditFlags = "rights",
                            EditSummary = "Needed",
                            AdditionalData = "from autoreviewer, extendedconfirmed, reviewer to autoreviewer, extendedconfirmed, reviewer, templateeditor"
                        });
                #endregion
                #region stable
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 config10 02 5* 03MelanieN 5*  10MelanieN configured pending changes settings for [[02Gabriel Batistuta10]] [Auto-accept: require \"autoconfirmed\" permission] (expires 00:10, 4 July 2018 (UTC)): Persistent addition of [[WP:INTREF|unsourced or poorly sourced content]]")
                    .Returns(
                        new RecentChange("MelanieN")
                        {
                            Log = "stable",
                            Page = "Gabriel Batistuta",
                            EditFlags = "config",
                            EditSummary = "Persistent addition of [[WP:INTREF|unsourced or poorly sourced content]]"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 config10 02 5* 03Vanamonde93 5*  10Vanamonde93 configured pending changes settings for [[02Brandon Victor Dixon10]] [Auto-accept: require \"autoconfirmed\" permission]: Reducing protection level per request on my talk page.")
                    .Returns(
                        new RecentChange("Vanamonde93")
                        {
                            Log = "stable",
                            Page = "Brandon Victor Dixon",
                            EditFlags = "config",
                            EditSummary = "Reducing protection level per request on my talk page."
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 config10 02 5* 03JzG 5*  10JzG configured pending changes settings for [[02Under Our Skin10]] [Auto-review: requires \"autoconfirmed\" permission]: Persistent [[WP:Disruptive editing|disruptive editing]]. Propaganda film promoting quack diagnosis, not many watchers, an indefinite risk for well-meaning newbies here to [[WP:RGW]]. ")
                    .Returns(
                        new RecentChange("JzG")
                        {
                            Log = "stable",
                            Page = "Under Our Skin",
                            EditFlags = "config",
                            EditSummary = "Persistent [[WP:Disruptive editing|disruptive editing]]. Propaganda film promoting quack diagnosis, not many watchers, an indefinite risk for well-meaning newbies here to [[WP:RGW]]. "
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 config10 02 5* 03Kelapstick 5*  10Kelapstick configured pending changes settings for [[02Get Out10]] [Auto-review: requires \"autoconfirmed\" permission] (expires 21:18, 17 June 2018 (UTC)): Persistent [[WP:Vandalism|vandalism]]")
                    .Returns(
                        new RecentChange("Kelapstick")
                        {
                            Log = "stable",
                            Page = "Get Out",
                            EditFlags = "config",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 reset10 02 5* 03Swarm 5*  10Swarm reset pending changes settings for [[Doug Ford Jr.]] (expires 20:03, 4 May 2018 (UTC)): Persistent [[WP:Sock puppetry|sock puppetry]] - increasing to longer-term semi-protection")
                    .Returns(
                        new RecentChange("Swarm")
                        {
                            Log = "stable",
                            Page = "Doug Ford Jr.",
                            EditFlags = "reset",
                            EditSummary = "Persistent [[WP:Sock puppetry|sock puppetry]] - increasing to longer-term semi-protection"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 move_stable10 02 5* 03Ollieinc 5*  10Ollieinc moved pending changes settings from [[Cartoon Network (Australia and New Zealand)]] to [[02Cartoon Network (Australian and New Zealand TV channel)10]]: [[Cartoon Network (Australia and New Zealand)]] moved to [[02Cartoon Network (Australian and New Zealand TV channel)10]]")
                    .Returns(
                        new RecentChange("Ollieinc")
                        {
                            Log = "stable",
                            Page = "Cartoon Network (Australia and New Zealand)",
                            TargetPage = "Cartoon Network (Australian and New Zealand TV channel)",
                            EditFlags = "move_stable"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/stable14]]4 modify10 02 5* 03Vanamonde93 5*  10Vanamonde93 altered pending changes settings for [[02IndiGo10]] [Auto-accept: require \"autoconfirmed\" permission]: Persistent [[WP:Disruptive editing|disruptive editing]]")
                    .Returns(
                        new RecentChange("Vanamonde93")
                        {
                            Log = "stable",
                            Page = "IndiGo",
                            EditFlags = "modify",
                            EditSummary = "Persistent [[WP:Disruptive editing|disruptive editing]]"
                        });
                #endregion
                #region tag
                yield return new TestCaseData(
                        "14[[07Special:Log/tag14]]4 update10 02 5* 03Xaosflux 5*  10Xaosflux added the tag AWB to log entry 90109097 of page [[02Special:Log/patrol10]]: tag test")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "tag",
                            EditFlags = "update",
                            Page = "Special:Log/patrol",
                            EditSummary = "tag test"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/tag14]]4 update10 02 5* 03Xaosflux 5*  10Xaosflux added the tag bot trial to revision 53987300 of page [[02User:Fluxbot10]]: test")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "tag",
                            EditFlags = "update",
                            Page = "User:Fluxbot",
                            EditSummary = "test"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/tag14]]4 update10 02 5* 03Xaosflux 5*  10Xaosflux removed the tag bot trial from revision 53987300 of page [[02User:Fluxbot10]]")
                    .Returns(
                        new RecentChange("Xaosflux")
                        {
                            Log = "tag",
                            EditFlags = "update",
                            Page = "User:Fluxbot"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/tag14]]4 update10 02 5* 03Qwerfjkl (bot) 5*  10Qwerfjkl (bot) added the tags huggle, ProveIt edit, WPCleaner, STiki, AWB, bot trial, Image up for deletion on Commons, WikiLoop Battlefield, discretionary, self-published source, self-published-blog to revision 1065689178 of page [[02User:Qwerfjkl/sandbox10]]")
                    .Returns(
                        new RecentChange("Qwerfjkl (bot)")
                        {
                            Log = "tag",
                            EditFlags = "update",
                            Page = "User:Qwerfjkl/sandbox"
                        });
                #endregion
                #region thanks
                yield return new TestCaseData(
                        "14[[07Special:Log/thanks14]]4 thank10 02 5* 03AuH2ORepublican 5*  10AuH2ORepublican thanked Eagleash")
                    .Returns(
                        new RecentChange("AuH2ORepublican")
                        {
                            Log = "thanks",
                            EditFlags = "thank",
                            TargetUser = "Eagleash"
                        });
                #endregion
                #region translationreview

                yield return new TestCaseData(
                        "14[[07Special:Log/translationreview14]]4 message10 02 5* 03Meno25 5*  10Meno25 reviewed translation [[02Translations:Interface editors/3/ar10]]")
                    .Returns(
                        new RecentChange("Meno25")
                        {
                            Log = "translationreview",
                            Page = "Translations:Interface editors/3/ar",
                            EditFlags = "message",
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/translationreview14]]4 group10 02 5* 03FuzzyBot 5*  10FuzzyBot changed the state of Arabic translations of [[Special:Translate/agg- High priority - Access to nonpublic information policy|Access to nonpublic information policy]] from Ready to Proofreading")
                    .Returns(
                        new RecentChange("FuzzyBot")
                        {
                            Log = "translationreview",
                            Page = "Access to nonpublic information policy",
                            EditFlags = "group"
                        });
                
                yield return new TestCaseData(
                        "\u000314[[\u000307Special:Log/translationreview\u000314]]\u00034 unfuzzy\u000310 \u000302\u0003 \u00035*\u0003 \u000303Mervat\u0003 \u00035*\u0003  \u000310Mervat marked translation [[\u000302Translations:Movement Charter/Glossary/30/ar\u000310]] as no longer outdated\u0003")
                    .Returns(
                        new RecentChange("Mervat")
                        {
                            Log = "translationreview",
                            Page = "Translations:Movement Charter/Glossary/30/ar",
                            EditFlags = "unfuzzy"
                        });
                #endregion
                #region upload
                yield return new TestCaseData(
                        "14[[07Special:Log/upload14]]4 overwrite10 02 5* 03In Memoriam A.H.H. 5*  10uploaded a new version of \"[[02File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png10]]\": Cropped")
                    .Returns(
                        new RecentChange("In Memoriam A.H.H.")
                        {
                            Log = "upload",
                            EditFlags = "overwrite",
                            Page = "File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png",
                            EditSummary = "Cropped"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/upload14]]4 overwrite10 02 5* 03In Memoria 5*  10uploaded a new version of \"[[02File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png10]]\"")
                    .Returns(
                        new RecentChange("In Memoria")
                        {
                            Log = "upload",
                            EditFlags = "overwrite",
                            Page = "File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png"
                        });    
                
                yield return new TestCaseData(
                        "14[[07Special:Log/upload14]]4 revert10 02 5* 03Mani.Ghv 5*  10uploaded a new version of \"[[02File:Paykan VC Logo.svg10]]\": Reverted to version as of 02:00, 25 February 2019 (UTC)")
                    .Returns(
                        new RecentChange("Mani.Ghv")
                        {
                            Log = "upload",
                            EditFlags = "revert",
                            Page = "File:Paykan VC Logo.svg",
                            EditSummary = "Reverted to version as of 02:00, 25 February 2019 (UTC)"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/upload14]]4 upload10 02 5* 03I Memoriam A.H.H. 5*  10uploaded \"[[02File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png10]]\": Cropped")
                    .Returns(
                        new RecentChange("I Memoriam A.H.H.")
                        {
                            Log = "upload",
                            EditFlags = "upload",
                            Page = "File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png",
                            EditSummary = "Cropped"
                        });
                
                // faked
                yield return new TestCaseData(
                        "14[[07Special:Log/upload14]]4 upload10 02 5* 03I Memoria 5*  10uploaded \"[[02File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png10]]\"")
                    .Returns(
                        new RecentChange("I Memoria")
                        {
                            Log = "upload",
                            EditFlags = "upload",
                            Page = "File:Columbia logo, from Concerto in B Flat Minor, circa 1942.png"
                        });
                #endregion
            }
        }
    }
}