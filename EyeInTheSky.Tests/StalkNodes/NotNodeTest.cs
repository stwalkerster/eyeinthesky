using EyeInTheSky.Model.Interfaces;
using Moq;
using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class NotNodeTest : SingleChildNodeTestBase<NotNode>
    {
        [Test, TestCaseSource(typeof(NotNodeTest), "TestCases")]
        public bool? DualOperatorTest(IStalkNode a)
        {
            var node = new NotNode();
            node.ChildNode = a;

            return node.Match(new RecentChange("", "", "", "", "", 0), false);
        }

        private static IEnumerable TestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);

                yield return new TestCaseData(new TrueNode()).Returns(false);
                yield return new TestCaseData(new FalseNode()).Returns(true);
                yield return new TestCaseData(nullNodeMock.Object).Returns(null);
            }
        }
    }
}