namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PageStalkNodeTest : RegexLeafNodeTestBase<PageStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder();
        }

        [Test]
        public void ShouldNotMatchNullSummary()
        {
            var node = new PageStalkNode();
            node.SetMatchExpression("abc");

            var rc = this.RecentChangeBuilder();
            rc.Page.Returns((string)null);

            var result = node.Match(rc);

            Assert.False(result);
        }

        [Test, TestCaseSource(typeof(PageStalkNodeTest), nameof(TestCases))]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }

        [Test]
        public void ShouldMatchMoveLogOutbound()
        {
            // arrange
            var pageNode = new PageStalkNode();
            pageNode.SetMatchExpression("foo");

            var l = Substitute.For<IRecentChange>();
            l.Log.Returns("move");
            l.User.Returns("user");
            l.EditFlags.Returns("move");
            l.Page.Returns("foo");
            l.TargetPage.Returns("bar");

            // act
            var result = pageNode.Match(l);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldMatchMoveLogInbound()
        {
            // arrange
            var pageNode = new PageStalkNode();
            pageNode.SetMatchExpression("bar");

            var l = Substitute.For<IRecentChange>();
            l.Log.Returns("move");
            l.User.Returns("user");
            l.EditFlags.Returns("move");
            l.Page.Returns("foo");
            l.TargetPage.Returns("bar");

            // act
            var result = pageNode.Match(l);

            // assert
            Assert.IsTrue(result);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                RegexLeafNode n = new PageStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n).Returns(true);
                n = new PageStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n).Returns(false);
                n = new PageStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new PageStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(false);
                n = new PageStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new PageStalkNode();
                n.SetMatchExpression("pqr");
                yield return new TestCaseData(n).Returns(false);
                n = new PageStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}