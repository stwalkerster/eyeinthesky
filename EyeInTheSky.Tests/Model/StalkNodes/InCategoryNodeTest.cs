namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class InCategoryNodeTest : LeafNodeTestBase<InCategoryNode>
    {
        private IRecentChange rc;
        private Mock<IMediaWikiApi> mwApi;

        [SetUp]
        public void LocalSetup()
        {
            this.rc = new RecentChange("abc", "def", "ghi", "jkl", "mno", 123);
            this.mwApi = new Mock<IMediaWikiApi>();
            this.rc.MediaWikiApi = this.mwApi.Object;
        }
        
        [Test, TestCaseSource(typeof(InCategoryNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node, bool value)
        {
            this.mwApi.Setup(x => x.PageIsInCategory(It.IsAny<string>(), It.IsAny<string>())).Returns(value);
            
            return node.Match(this.rc);
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