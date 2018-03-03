using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class UserStalkNodeTest
    {
        private RecentChange rc;

        [SetUp]
        public void Setup()
        {
            this.rc = new RecentChange("abc", "def", "ghi", "jkl", "mno", 123);
        }
        
        [Test, TestCaseSource(typeof(UserStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                LeafNode n = new UserStalkNode();
                n.setMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.setMatchExpression("def");
                yield return new TestCaseData(n).Returns(true);
                n = new UserStalkNode();
                n.setMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.setMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.setMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.setMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}