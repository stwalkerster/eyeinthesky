namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System;
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ExpiryNodeTest : SingleChildNodeTestBase<ExpiryNode>
    {
        [Test, TestCaseSource(typeof(ExpiryNodeTest), "ExpiredTestCases")]
        public bool? ExpiredOperatorTest(IStalkNode a)
        {
            var node = new ExpiryNode
            {
                Expiry = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0))
            };
            
            node.ChildNode = a;

            return node.Match(new RecentChange(""), false);
        }

        [Test, TestCaseSource(typeof(ExpiryNodeTest), "NonExpiredTestCases")]
        public bool? NonExpiredOperatorTest(IStalkNode a)
        {
            var node = new ExpiryNode
            {
                Expiry = DateTime.UtcNow.Add(new TimeSpan(0, 1, 0))
            };
            
            node.ChildNode = a;

            return node.Match(new RecentChange(""), false);
        }
        
        private static IEnumerable NonExpiredTestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);

                yield return new TestCaseData(new TrueNode()).Returns(true);
                yield return new TestCaseData(new FalseNode()).Returns(false);
                yield return new TestCaseData(nullNodeMock.Object).Returns(null);
            }
        }
        
        private static IEnumerable ExpiredTestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);

                yield return new TestCaseData(new TrueNode()).Returns(false);
                yield return new TestCaseData(new FalseNode()).Returns(false);
                yield return new TestCaseData(nullNodeMock.Object).Returns(false);
            }
        }
    }
}