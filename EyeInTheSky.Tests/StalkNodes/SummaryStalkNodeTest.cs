using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class SummaryStalkNodeTest
    {
        private RecentChange rc;

        [SetUp]
        public void Setup()
        {
            this.rc = new RecentChange("abc", "def", "ghi", "jkl", "mno", 123);
        }
        
        [Test, TestCaseSource(typeof(SummaryStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                LeafNode n = new SummaryStalkNode();
                n.setMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.setMatchExpression("def");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.setMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.setMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(true);
                n = new SummaryStalkNode();
                n.setMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new SummaryStalkNode();
                n.setMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}