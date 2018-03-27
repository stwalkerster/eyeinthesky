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
    public class AndNodeTest : MultiChildNodeTestBase<AndNode>
    {   
        [Test, TestCaseSource(typeof(AndNodeTest), "TestCases")]
        public bool? MultiOperatorTest(List<IStalkNode> a)
        {
            var node = new AndNode();
            node.ChildNodes.AddRange(a);

            return node.Match(new RecentChange("", "", "", "", "", 0), false);
        }

        private static IEnumerable TestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), new TrueNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), new FalseNode()}).Returns(false);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), nullNodeMock.Object}).Returns(null);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), nullNodeMock.Object}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock.Object, new TrueNode()}).Returns(null);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock.Object, new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock.Object, nullNodeMock.Object}).Returns(null);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new TrueNode(), new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(),new FalseNode(), new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(),new FalseNode(), new TrueNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock.Object}).Returns(null);
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
            
            var node = new AndNode();
            node.ChildNodes.Add(new FalseNode());
            
            var nodeRightChildNode = new UserGroupStalkNode();
            nodeRightChildNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(nodeRightChildNode);
            
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
            
            var node = new AndNode();
            node.ChildNodes.Add(new TrueNode());
            
            var nodeRightChildNode = new UserGroupStalkNode();
            nodeRightChildNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(nodeRightChildNode);
            
            // act
            node.Match(rc);

            // assert
            mwApi.Verify(x => x.GetUserGroups(It.IsAny<string>()), Times.Once);
        }
    }
}