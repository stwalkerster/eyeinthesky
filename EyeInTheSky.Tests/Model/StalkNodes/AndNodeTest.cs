namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AndNodeTest : MultiChildNodeTestBase<AndNode>
    {   
        [Test, TestCaseSource(typeof(AndNodeTest), "TestCases")]
        public bool? MultiOperatorTest(List<IStalkNode> a)
        {
            var node = new AndNode();
            node.ChildNodes.AddRange(a);

            return node.Match(new RecentChange(""), false);
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
            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.GetUserGroups()).Returns(new List<string> {"user", "*"});
            
            var node = new AndNode();
            node.ChildNodes.Add(new FalseNode());
            
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);
            
            // act
            node.Match(rc.Object);

            // assert
            rc.Verify(x => x.GetUserGroups(), Times.Never);
        }

        [Test]
        public void LazyEvaluationInverseSkipTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.GetUserGroups()).Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            node.ChildNodes.Add(new FalseNode());

            // act
            node.Match(rc.Object);

            // assert
            rc.Verify(x => x.GetUserGroups(), Times.Never);
        }

        [Test]
        public void LazyEvaluationForceTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.GetUserGroups()).Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            node.ChildNodes.Add(new TrueNode());

            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            // act
            node.Match(rc.Object);

            // assert
            rc.Verify(x => x.GetUserGroups(), Times.Once);
        }

        [Test]
        public void LazyEvaluationInverseForceTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.Setup(x => x.GetUserGroups()).Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            node.ChildNodes.Add(new TrueNode());

            // act
            node.Match(rc.Object);

            // assert
            rc.Verify(x => x.GetUserGroups(), Times.Once);
        }
    }
}