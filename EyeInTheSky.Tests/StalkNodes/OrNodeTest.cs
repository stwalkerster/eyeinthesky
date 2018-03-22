using EyeInTheSky.Model.Interfaces;
using Moq;
using NUnit.Framework;

namespace EyeInTheSky.Tests.StalkNodes
{
    using System.Collections;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.StalkNodes;

    [TestFixture]
    public class OrNodeTest : DoubleChildNodeTestBase<OrNode>
    {
        [Test, TestCaseSource(typeof(OrNodeTest), "TestCases")]
        public bool? DualOperatorTest(IStalkNode a, IStalkNode b)
        {
            var node = new OrNode();
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

                yield return new TestCaseData(new TrueNode(), new TrueNode()).Returns(true);
                yield return new TestCaseData(new FalseNode(), new TrueNode()).Returns(true);
                yield return new TestCaseData(new TrueNode(), new FalseNode()).Returns(true);
                yield return new TestCaseData(new FalseNode(), new FalseNode()).Returns(false);

                yield return new TestCaseData(new TrueNode(), nullNodeMock.Object).Returns(true);
                yield return new TestCaseData(new FalseNode(), nullNodeMock.Object).Returns(null);
                yield return new TestCaseData(nullNodeMock.Object, new TrueNode()).Returns(true);
                yield return new TestCaseData(nullNodeMock.Object, new FalseNode()).Returns(null);
                yield return new TestCaseData(nullNodeMock.Object, nullNodeMock.Object).Returns(null);
            }
        }

        [Test]
        public void LazyEvaluationSkipTest()
        {
            // arrange
            var mwApi = new Mock<IMediaWikiApi>();
            mwApi.Setup(x => x.GetUserGroups(It.IsAny<string>())).Returns(new List<string> {"user", "*"});
            
            var rc = new RecentChange("a", "b", "c", "d", "e", 0);
            rc.MediaWikiApi = mwApi.Object;
            
            var node = new OrNode();
            node.LeftChildNode = new TrueNode();
            
            var nodeRightChildNode = new UserGroupStalkNode();
            nodeRightChildNode.SetMatchExpression("sysop");
            node.RightChildNode = nodeRightChildNode;
            
            // act
            node.Match(rc);

            // assert
            mwApi.Verify(x => x.GetUserGroups(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void LazyEvaluationForceTest()
        {
            // arrange
            var mwApi = new Mock<IMediaWikiApi>();
            mwApi.Setup(x => x.GetUserGroups(It.IsAny<string>())).Returns(new List<string> {"user", "*"});
            
            var rc = new RecentChange("a", "b", "c", "d", "e", 0);
            rc.MediaWikiApi = mwApi.Object;
            
            var node = new OrNode();
            node.LeftChildNode = new FalseNode();
            
            var nodeRightChildNode = new UserGroupStalkNode();
            nodeRightChildNode.SetMatchExpression("sysop");
            node.RightChildNode = nodeRightChildNode;
            
            // act
            node.Match(rc);

            // assert
            mwApi.Verify(x => x.GetUserGroups(It.IsAny<string>()), Times.Once);
        }
    }
}