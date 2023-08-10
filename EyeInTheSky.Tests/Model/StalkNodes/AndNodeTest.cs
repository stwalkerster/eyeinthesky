namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AndNodeTest : MultiChildNodeTestBase<AndNode>
    {   
        [Test, TestCaseSource(typeof(AndNodeTest), nameof(TestCases))]
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
                var nullNodeMock = Substitute.For<IStalkNode>();
                nullNodeMock.Match(Arg.Any<IRecentChange>(), false).Returns((bool?)null);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), new TrueNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), new FalseNode()}).Returns(false);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), nullNodeMock}).Returns(null);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(), nullNodeMock}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock, new TrueNode()}).Returns(null);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock, new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock, nullNodeMock}).Returns(null);
                
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode(), new TrueNode(), new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(),new FalseNode(), new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode(),new FalseNode(), new TrueNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {new TrueNode()}).Returns(true);
                yield return new TestCaseData(new List<IStalkNode> {new FalseNode()}).Returns(false);
                yield return new TestCaseData(new List<IStalkNode> {nullNodeMock}).Returns(null);
            }
        }

        [Test]
        public void LazyEvaluationSkipTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.GetUserGroups().Returns(new List<string> {"user", "*"});
            
            var node = new AndNode();
            node.ChildNodes.Add(new FalseNode());
            
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);
            
            // act
            node.Match(rc);

            // assert
            rc.DidNotReceive().GetUserGroups();
        }

        [Test]
        public void LazyEvaluationInverseSkipTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.GetUserGroups().Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            node.ChildNodes.Add(new FalseNode());

            // act
            node.Match(rc);

            // assert
            rc.DidNotReceive().GetUserGroups();
        }

        [Test]
        public void LazyEvaluationForceTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.GetUserGroups().Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            node.ChildNodes.Add(new TrueNode());

            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            // act
            node.Match(rc);

            // assert
            rc.Received(1).GetUserGroups();
        }

        [Test]
        public void LazyEvaluationInverseForceTest()
        {
            // arrange
            var rc = this.RecentChangeBuilder();
            rc.GetUserGroups().Returns(new List<string> {"user", "*"});

            var node = new AndNode();
            var lazyNode = new UserGroupStalkNode();
            lazyNode.SetMatchExpression("sysop");
            node.ChildNodes.Add(lazyNode);

            node.ChildNodes.Add(new TrueNode());

            // act
            node.Match(rc);

            // assert
            rc.Received(1).GetUserGroups();
        }
    }
}