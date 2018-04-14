using System.Collections;
using EyeInTheSky.Model.Interfaces;
using EyeInTheSky.Model.StalkNodes;
using EyeInTheSky.Model.StalkNodes.BaseNodes;
using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;

namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using NUnit.Framework;

    [TestFixture]
    public class LogStalkNodeTest : RegexLeafNodeTestBase<LogStalkNode>
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
            var node = new LogStalkNode();
            node.SetMatchExpression("abc");

            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.Log).Returns<string>(null);

            var result = node.Match(rc.Object);

            Assert.False(result);
        }

        [Test, TestCaseSource(typeof(LogStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                RegexLeafNode n = new LogStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new LogStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n).Returns(false);
                n = new LogStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new LogStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(false);
                n = new LogStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new LogStalkNode();
                n.SetMatchExpression("pqr");
                yield return new TestCaseData(n).Returns(true);
                n = new LogStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}
