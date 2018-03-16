using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System;
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class SummaryStalkNodeTest : LeafNodeTestBase<SummaryStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = new RecentChange("abc", "def", "ghi", "jkl", "mno", 123);
        }
        
        [Test, TestCaseSource(typeof(SummaryStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                LeafNode n = new SummaryStalkNode();
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