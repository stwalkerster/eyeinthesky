namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class UserStalkNodeTest : RegexLeafNodeTestBase<UserStalkNode>
    {
        private IRecentChange rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder();
        }

        [Test]
        public void ShouldMatchBlockLog()
        {
            // arrange
            var userNode = new UserStalkNode();
            userNode.SetMatchExpression("bar");

            var l = Substitute.For<IRecentChange>();
            l.Log.Returns("block");
            l.User.Returns("user");
            l.EditFlags.Returns("block");
            l.TargetUser.Returns("bar");

            // act
            var result = userNode.Match(l);

            // assert
            Assert.IsTrue(result);
        }

        [Test, TestCaseSource(typeof(UserStalkNodeTest), nameof(TestCases))]
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
                n = new UserStalkNode();
                n.SetMatchExpression("alttgt");
                yield return new TestCaseData(n).Returns(true);
            }
        }  
    }
}