namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NUnit.Framework;

    [TestFixture]
    public class FlagStalkNodeTest : RegexLeafNodeTestBase<FlagStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder().Object;
        }

        [Test]
        public void ShouldNotMatchNullSummary()
        {
            var node = new FlagStalkNode();
            node.SetMatchExpression("abc");

            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.EditFlags).Returns<string>(null);

            var result = node.Match(rc.Object);

            Assert.False(result);
        }

        [Test, TestCaseSource(typeof(FlagStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                RegexLeafNode n = new FlagStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new FlagStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n).Returns(false);
                n = new FlagStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new FlagStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(false);
                n = new FlagStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n).Returns(true);
                n = new FlagStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}