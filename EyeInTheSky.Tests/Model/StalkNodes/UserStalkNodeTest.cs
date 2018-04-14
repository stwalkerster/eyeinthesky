namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class UserStalkNodeTest : RegexLeafNodeTestBase<UserStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder().Object;
        }

        [Test]
        public void ShouldMatchBlockLog()
        {
            // arrange
            var userNode = new UserStalkNode();
            userNode.SetMatchExpression("bar");

            var l = new Mock<IRecentChange>();
            l.Setup(x => x.Log).Returns("block");
            l.Setup(x => x.User).Returns("user");
            l.Setup(x => x.EditFlags).Returns("block");
            l.Setup(x => x.TargetUser).Returns("bar");

            // act
            var result = userNode.Match(l.Object);

            // assert
            Assert.IsTrue(result);
        }

        [Test, TestCaseSource(typeof(UserStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node)
        {
            return node.Match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                RegexLeafNode n = new UserStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n).Returns(true);
                n = new UserStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.SetMatchExpression("pqr");
                yield return new TestCaseData(n).Returns(false);
                n = new UserStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n).Returns(false);
            }
        }  
    }
}