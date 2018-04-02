namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class NotNodeTest : SingleChildNodeTestBase<NotNode>
    {
        [Test, TestCaseSource(typeof(NotNodeTest), "TestCases")]
        public bool? DualOperatorTest(IStalkNode a)
        {
            var node = new NotNode();
            node.ChildNode = a;

            return node.Match(new RecentChange(""), false);
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