namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class PageStalkNodeTest : RegexLeafNodeTestBase<PageStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder().Object;
        }

        [Test, TestCaseSource(typeof(PageStalkNodeTest), "TestCases")]
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

            var l = new Mock<IRecentChange>();
            l.Setup(x => x.Log).Returns("move");
            l.Setup(x => x.User).Returns("user");
            l.Setup(x => x.EditFlags).Returns("move");
            l.Setup(x => x.Page).Returns("foo");
            l.Setup(x => x.TargetPage).Returns("bar");

            // act
            var result = pageNode.Match(l.Object);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldMatchMoveLogInbound()
        {
            // arrange
            var pageNode = new PageStalkNode();
            pageNode.SetMatchExpression("bar");

            var l = new Mock<IRecentChange>();
            l.Setup(x => x.Log).Returns("move");
            l.Setup(x => x.User).Returns("user");
            l.Setup(x => x.EditFlags).Returns("move");
            l.Setup(x => x.Page).Returns("foo");
            l.Setup(x => x.TargetPage).Returns("bar");

            // act
            var result = pageNode.Match(l.Object);

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
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}