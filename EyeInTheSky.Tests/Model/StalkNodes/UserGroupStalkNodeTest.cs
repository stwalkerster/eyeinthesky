namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class UserGroupStalkNodeTest : LeafNodeTestBase<UserGroupStalkNode>
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
        
        [Test, TestCaseSource(typeof(UserGroupStalkNodeTest), "TestCases")]
        public bool TestMatch(StalkNode node, List<string> groups)
        {
            this.mwApi.Setup(x => x.GetUserGroups(It.IsAny<string>())).Returns(groups);
            
            return node.Match(this.rc);
        }
        
        private static IEnumerable TestCases
        {
            get
            {
                LeafNode n;

                n = new UserGroupStalkNode();
                n.SetMatchExpression("abc");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                n = new UserGroupStalkNode();
                n.SetMatchExpression("def");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                n = new UserGroupStalkNode();
                n.SetMatchExpression("ghi");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                n = new UserGroupStalkNode();
                n.SetMatchExpression("jkl");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                n = new UserGroupStalkNode();
                n.SetMatchExpression("mno");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                n = new UserGroupStalkNode();
                n.SetMatchExpression("123");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                
                n = new UserGroupStalkNode();
                n.SetMatchExpression("user");
                yield return new TestCaseData(n, new List<string>()).Returns(false);
                
                n = new UserGroupStalkNode();
                n.SetMatchExpression("user");
                yield return new TestCaseData(n, new List<string>{"user"}).Returns(true);
                
                n = new UserGroupStalkNode();
                n.SetMatchExpression("checkuser");
                yield return new TestCaseData(n, new List<string> {"user", "sysop", "*", "autoconfirmed"})
                    .Returns(false);
            }
        }
    }
}