using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class AndNodeTest
    {
        [Test, TestCaseSource(typeof(AndNodeTest), "TestCases")]
        public bool DualOperatorTest(StalkNode a, StalkNode b)
        {
            var node = new AndNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            return node.match(new RecentChange("", "", "", "", "", 0));
        }

        private static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new TrueNode(), new TrueNode()).Returns(true);
                yield return new TestCaseData(new FalseNode(), new TrueNode()).Returns(false);
                yield return new TestCaseData(new TrueNode(), new FalseNode()).Returns(false);
                yield return new TestCaseData(new FalseNode(), new FalseNode()).Returns(false);
            }
        }
    }
}