using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class NotNodeTest
    {
        [Test, TestCaseSource(typeof(NotNodeTest), "TestCases")]
        public bool DualOperatorTest(StalkNode a)
        {
            var node = new NotNode();
            node.ChildNode = a;

            return node.match(new RecentChange("", "", "", "", "", 0));
        }

        private static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new TrueNode()).Returns(false);
                yield return new TestCaseData(new FalseNode()).Returns(true);
            }
        }
    }
}