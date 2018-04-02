namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class RecentChangeParserTests : TestBase
    {
        private RecentChangeParser rcparser;

        [SetUp]
        public void LocalSetup()
        {
            this.rcparser = new RecentChangeParser(this.LoggerMock.Object, null);
        }

        [Test, TestCaseSource(typeof(RecentChangeParserTests), "ParseTestData")]
        public IRecentChange ShouldParseCorrectly(string data)
        {
            var shouldParseCorrectly = this.rcparser.Parse(data);
            this.LoggerMock.Verify(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
            return shouldParseCorrectly;
        }

        [Test, TestCaseSource(typeof(RecentChangeParserTests), "ParseLogTestData")]
        public IRecentChange ShouldParseLogCorrectly(string data)
        {
            var shouldParseLogCorrectly = this.rcparser.Parse(data);
            this.LoggerMock.Verify(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never, "Failed to parse log entry");
            return shouldParseLogCorrectly;
        }

        [Test]
        public void ShouldErrorIfUnknownLogType()
        {
            // arrange
            string data = "14[[07Special:Log/fakelog14]]4 sdfsdf10 02 5* 03Jimbo 5*  10Loggy message";

            // act
            this.rcparser.Parse(data);

            // assert
            this.LoggerMock.Verify(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
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
                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 hit10 02 5* 03Ceanneisenhammer 5*  10Ceanneisenhammer triggered [[Special:AbuseFilter/527|filter 527]], performing the action \"createaccount\" on [[02Special:UserLogin10]]. Actions taken: none ([[Special:AbuseLog/20724740|details]])")
                    .Returns(
                        new RecentChange("Ceanneisenhammer")
                        {
                            Page = "Special:Log/abusefilter",
                            EditFlags = "hit",
                            Log = "abusefilter",
                            EditSummary = "Ceanneisenhammer triggered [[Special:AbuseFilter/527|filter 527]], performing the action \"createaccount\" on [[Special:UserLogin]]. Actions taken: none ([[Special:AbuseLog/20724740|details]])"
                        }).Ignore("Not sure what to do with this, as it embeds a different action within");

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
                        "14[[07Special:Log/pagetriage-curation14]]4 unreviewed10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Veronica Cool10]] as unreviewed")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-curation",
                            Page = "Veronica Cool",
                            EditFlags = "unreviewed"
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
                        "14[[07Special:Log/pagetriage-deletion14]]4 delete10 02 5* 03SamHolt6 5*  10SamHolt6 marked [[02Ak husky10]] for deletion with db-band tag")
                    .Returns(
                        new RecentChange("SamHolt6")
                        {
                            Log = "pagetriage-deletion",
                            Page = "Ak husky",
                            EditFlags = "delete"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/patrol14]]4 patrol10 02 5* 03SshibumXZ 5*  10marked revision 833886654 of [[02Sekyiwa Shakur10]] patrolled ")
                    .Returns(
                        new RecentChange("SshibumXZ")
                        {
                            Log = "patrol",
                            Page = "Sekyiwa Shakur",
                            EditFlags = "patrol"
                        });
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 protect10 02 5* 03Ohnoitsjamie 5*  10protected \"[[Alison Brie ‎[edit=autoconfirmed] (expires 20:56, 2 October 2018 (UTC))‎[move=autoconfirmed] (expires 20:56, 2 October 2018 (UTC))]]\": Persistent [[WP:Vandalism|vandalism]]")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Alison Brie",
                            EditFlags = "protect",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]"
                        }).Ignore("Doesn't work");
                
                yield return new TestCaseData(
                        "14[[07Special:Log/protect14]]4 modify10 02 5* 03Ohnoitsjamie 5*  10changed protection level of Tom Kenny filmography ‎[edit=autoconfirmed] (expires 20:59, 2 October 2018 (UTC))‎[move=autoconfirmed] (expires 20:59, 2 October 2018 (UTC)): Persistent [[WP:Vandalism|vandalism]]: IP hopping crap")
                    .Returns(
                        new RecentChange("Ohnoitsjamie")
                        {
                            Log = "protect",
                            Page = "Tom Kenny filmography",
                            EditFlags = "modify",
                            EditSummary = "Persistent [[WP:Vandalism|vandalism]]: IP hopping crap"
                        }).Ignore("Doesn't work");
                
                yield return new TestCaseData(
                        "14[[07Special:Log/renameuser14]]4 renameuser10 02 5* 03Céréales Killer 5*  10Céréales Killer renamed user [[02User:SujaiRamPrasathC10]] (0 edits) to [[User:ZszasdojcqSsadaS]]: per [[m:Special:GlobalRenameQueue/request/41529|request]]")
                    .Returns(
                        new RecentChange("Céréales Killer")
                        {
                            Log = "renameuser",
                            TargetUser = "ZszasdojcqSsadaS",
                            EditFlags = "renameuser",
                            EditSummary = "per [[m:Special:GlobalRenameQueue/request/41529|request]]"
                        }).Ignore("Three users here...");
                
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
                        "14[[07Special:Log/thanks14]]4 thank10 02 5* 03AuH2ORepublican 5*  10AuH2ORepublican thanked Eagleash")
                    .Returns(
                        new RecentChange("AuH2ORepublican")
                        {
                            Log = "thanks",
                            EditFlags = "thank",
                            TargetUser = "Eagleash"
                        });

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
            }
        }
    }
}