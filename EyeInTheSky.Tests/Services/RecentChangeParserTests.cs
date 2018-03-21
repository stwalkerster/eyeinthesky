namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using NUnit.Framework;

    [TestFixture]
    public class RecentChangeParserTests : TestBase
    {
        private RecentChangeParser rcparser;

        [SetUp]
        public void LocalSetup()
        {
            this.rcparser = new RecentChangeParser(this.LoggerMock.Object);
        }

        [Test, TestCaseSource(typeof(RecentChangeParserTests), "ParseTestData")]
        public IRecentChange ShouldParseCorrectly(string data)
        {
            return this.rcparser.Parse(data);
        }

        public static IEnumerable<TestCaseData> ParseTestData
        {
            get
            {
                yield return new TestCaseData(
                        "14[[07List of Sites of Special Scientific Interest in Kent14]]4 10 02https://en.wikipedia.org/w/index.php?diff=831707969&oldid=831707719 5* 03Dudley Miles 5* (+2) 10")
                    .Returns(
                        new RecentChange(
                            page: "List of Sites of Special Scientific Interest in Kent",
                            user: "Dudley Miles",
                            url: "https://en.wikipedia.org/w/index.php?diff=831707969&oldid=831707719",
                            editsummary: "",
                            flags: "",
                            sizediff: 2));

                yield return new TestCaseData(
                        "14[[07Matthew Lodge14]]4 M10 02https://en.wikipedia.org/w/index.php?diff=831708179&oldid=831696958 5* 03Doctorhawkes 5* (+45) 10Reverted edits by [[Special:Contribs/122.108.247.131|122.108.247.131]] ([[User talk:122.108.247.131|talk]]) to last version by Fleets")
                    .Returns(
                        new RecentChange(
                            page: "Matthew Lodge",
                            user: "Doctorhawkes",
                            url: "https://en.wikipedia.org/w/index.php?diff=831708179&oldid=831696958",
                            editsummary: "Reverted edits by [[Special:Contribs/122.108.247.131|122.108.247.131]] ([[User talk:122.108.247.131|talk]]) to last version by Fleets",
                            flags: "M",
                            sizediff: 45));

                yield return new TestCaseData(
                        "14[[07Myrmecia gratiosa14]]4 MB10 02https://en.wikipedia.org/w/index.php?diff=831707968&oldid=804753497 5* 03Tom.Bot 5* (+29) 10[[User:Tom.Bot/Task3|Task 3]]: +{{Taxonbar|[[:Category:Taxonbar templates without from parameter|from]]=[[d:Special:EntityPage/Q13868803|Q13868803]]}} ([[WT:TREE#Taxonbar addition requirements|2 sig. taxon IDs]]); WP:GenFix using [[Project:AWB|AWB]]")
                    .Returns(
                        new RecentChange(
                            page: "Myrmecia gratiosa",
                            user: "Tom.Bot",
                            url: "https://en.wikipedia.org/w/index.php?diff=831707968&oldid=804753497",
                            editsummary: "[[User:Tom.Bot/Task3|Task 3]]: +{{Taxonbar|[[:Category:Taxonbar templates without from parameter|from]]=[[d:Special:EntityPage/Q13868803|Q13868803]]}} ([[WT:TREE#Taxonbar addition requirements|2 sig. taxon IDs]]); WP:GenFix using [[Project:AWB|AWB]]",
                            flags: "MB",
                            sizediff: 29));

                yield return new TestCaseData(
                        "14[[07User:Erodr017/be bold14]]4 !N10 02https://en.wikipedia.org/w/index.php?oldid=831708535&rcid=1038874003 5* 03Erodr017 5* (+38) 10automatic post as part of sandbox guided tour")
                    .Returns(
                        new RecentChange(
                            page: "User:Erodr017/be bold",
                            user: "Erodr017",
                            url: "https://en.wikipedia.org/w/index.php?oldid=831708535&rcid=1038874003",
                            editsummary: "automatic post as part of sandbox guided tour",
                            flags: "!N",
                            sizediff: 38));

                yield return new TestCaseData(
                        "14[[07Special:Log/abusefilter14]]4 hit10 02 5* 03Ceanneisenhammer 5*  10Ceanneisenhammer triggered [[Special:AbuseFilter/527|filter 527]], performing the action \"createaccount\" on [[02Special:UserLogin10]]. Actions taken: none ([[Special:AbuseLog/20724740|details]])")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/abusefilter",
                            user: "Ceanneisenhammer",
                            url: "",
                            editsummary: "Ceanneisenhammer triggered [[Special:AbuseFilter/527|filter 527]], performing the action \"createaccount\" on [[Special:UserLogin]]. Actions taken: none ([[Special:AbuseLog/20724740|details]])",
                            flags: "hit",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/block14]]4 block10 02 5* 03MSGJ 5*  10blocked User:142.196.0.207 (account creation blocked) with an expiry time of 31 hours: [[WP:Edit warring|Edit warring]]")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/block",
                            user: "MSGJ",
                            url: "",
                            editsummary: "blocked User:142.196.0.207 (account creation blocked) with an expiry time of 31 hours: [[WP:Edit warring|Edit warring]]",
                            flags: "block",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 revision10 02 5* 03Hut 8.5 5*  10Hut 8.5 changed visibility of 5 revisions on page [[02Humza Yousaf10]]: content hidden: [[WP:RD1|RD1]]: Copyright violations: https://inews.co.uk/news/politics/humza-yousaf-scottish-transport-minister-facing-5000-fine-driving-uninsured/")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/delete",
                            user: "Hut 8.5",
                            url: "",
                            editsummary: "Hut 8.5 changed visibility of 5 revisions on page [[Humza Yousaf]]: content hidden: [[WP:RD1|RD1]]: Copyright violations: https://inews.co.uk/news/politics/humza-yousaf-scottish-transport-minister-facing-5000-fine-driving-uninsured/",
                            flags: "revision",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 delete10 02 5* 03Maile66 5*  10deleted \"[[02Talk:How to possible 50 Mbps speed in Just Rs.5000 per Year10]]\": [[WP:CSD#G8|G8]]: Page dependent on a deleted or nonexistent page")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/delete",
                            user: "Maile66",
                            url: "",
                            editsummary: "deleted \"[[Talk:How to possible 50 Mbps speed in Just Rs.5000 per Year]]\": [[WP:CSD#G8|G8]]: Page dependent on a deleted or nonexistent page",
                            flags: "delete",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/delete14]]4 delete_redir10 02 5* 03WikiOriginal-9 5*  10WikiOriginal-9 deleted redirect [[02David Carr (American football)10]] by overwriting: [[WP:CSD#G6|G6]]: Deleted to make way for move")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/delete",
                            user: "WikiOriginal-9",
                            url: "",
                            editsummary: "WikiOriginal-9 deleted redirect [[David Carr (American football)]] by overwriting: [[WP:CSD#G6|G6]]: Deleted to make way for move",
                            flags: "delete_redir",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/massmessage14]]4 skipnouser10 02 5* 03MediaWiki message delivery 5*  10Delivery of \"Project Tiger Writing Contest\" to [[02User talk:TiruTiruTiru10]] was skipped because the user account does not exist")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/massmessage",
                            user: "MediaWiki message delivery",
                            url: "",
                            editsummary: "Delivery of \"Project Tiger Writing Contest\" to [[User talk:TiruTiruTiru]] was skipped because the user account does not exist",
                            flags: "skipnouser",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move10 02 5* 03JaJaWa 5*  10moved [[02Yuanling Station10]] to [[Yuanling station]]")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/move",
                            user: "JaJaWa",
                            url: "",
                            editsummary: "moved [[Yuanling Station]] to [[Yuanling station]]",
                            flags: "move",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/move14]]4 move_redir10 02 5* 03WikiOriginal-9 5*  10moved [[02David Carr (quarterback)10]] to [[David Carr (American football)]] over redirect: no other American football player named David Carr")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/move",
                            user: "WikiOriginal-9",
                            url: "",
                            editsummary: "moved [[David Carr (quarterback)]] to [[David Carr (American football)]] over redirect: no other American football player named David Carr",
                            flags: "move_redir",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 create10 02 5* 03PiliPili Alex 5*  10New user account")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/newusers",
                            user: "PiliPili Alex",
                            url: "",
                            editsummary: "New user account",
                            flags: "create",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/newusers14]]4 create210 02 5* 03Esthenia 5*  10created new account User:EstheniaOG")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/newusers",
                            user: "Esthenia",
                            url: "",
                            editsummary: "created new account User:EstheniaOG",
                            flags: "create2",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/pagetriage-curation14]]4 reviewed10 02 5* 03Natureium 5*  10Natureium marked [[02John Thomas (Republican advertising)10]] as reviewed")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/pagetriage-curation",
                            user: "Natureium",
                            url: "",
                            editsummary: "Natureium marked [[John Thomas (Republican advertising)]] as reviewed",
                            flags: "reviewed",
                            sizediff: 0));

                yield return new TestCaseData(
                        "14[[07Special:Log/thanks14]]4 thank10 02 5* 03AuH2ORepublican 5*  10AuH2ORepublican thanked Eagleash")
                    .Returns(
                        new RecentChange(
                            page: "Special:Log/thanks",
                            user: "AuH2ORepublican",
                            url: "",
                            editsummary: "AuH2ORepublican thanked Eagleash",
                            flags: "thank",
                            sizediff: 0));
            }
        }
    }
}