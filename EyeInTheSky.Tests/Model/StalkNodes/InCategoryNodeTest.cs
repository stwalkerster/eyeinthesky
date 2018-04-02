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
    public class InCategoryNodeTest : LeafNodeTestBase<InCategoryNode>
    {
        private Mock<IRecentChange> rc;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = this.RecentChangeBuilder();
        }

        [Test, TestCaseSource(typeof(InCategoryNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node, bool value)
        {
            this.rc.Setup(x => x.PageIsInCategory(It.IsAny<string>())).Returns(value);

            return node.Match(this.rc.Object);
        }

        private static IEnumerable TestCases
        {
            get
            {
                LeafNode n;

                n = new InCategoryNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n, true).Returns(true);
                n = new InCategoryNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n, false).Returns(false);
            }
        }
    }
}