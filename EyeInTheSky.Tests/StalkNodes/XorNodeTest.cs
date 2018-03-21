using EyeInTheSky.Model.Interfaces;
using Moq;
using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class XorNodeTest : DoubleChildNodeTestBase<XorNode>
    {
        [Test, TestCaseSource(typeof(XorNodeTest), "TestCases")]
        public bool? DualOperatorTest(IStalkNode a, IStalkNode b)
        {
            var node = new XorNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            return node.Match(new RecentChange("", "", "", "", "", 0), false);
        }

        private static IEnumerable TestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);

                yield return new TestCaseData(new TrueNode(), new TrueNode()).Returns(false);
                yield return new TestCaseData(new FalseNode(), new TrueNode()).Returns(true);
                yield return new TestCaseData(new TrueNode(), new FalseNode()).Returns(true);
                yield return new TestCaseData(new FalseNode(), new FalseNode()).Returns(false);
                
                yield return new TestCaseData(new TrueNode(), nullNodeMock.Object).Returns(null);
                yield return new TestCaseData(new FalseNode(), nullNodeMock.Object).Returns(null);
                yield return new TestCaseData(nullNodeMock.Object, new TrueNode()).Returns(null);
                yield return new TestCaseData(nullNodeMock.Object, new FalseNode()).Returns(null);
                yield return new TestCaseData(nullNodeMock.Object, nullNodeMock.Object).Returns(null);
            }
        }
    }
}