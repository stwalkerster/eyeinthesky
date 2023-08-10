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
    public class SummaryStalkNodeTest : RegexLeafNodeTestBase<SummaryStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder();
        }

        [Test, TestCaseSource(typeof(SummaryStalkNodeTest), nameof(TestCases))]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }

        [Test]
        public void ShouldNotMatchNullSummary()
        {
            var node = new SummaryStalkNode();
            node.SetMatchExpression("abc");

            var rc = this.RecentChangeBuilder();
            rc.EditSummary.Returns((string)null);

            var result = node.Match(rc);

            Assert.False(result);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                RegexLeafNode n = new SummaryStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(true);
                n = new SummaryStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}