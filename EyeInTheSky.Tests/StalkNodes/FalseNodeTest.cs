namespace EyeInTheSky.Tests.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class FalseNodeTest : LogicalNodeTestBase<FalseNode>
    {
        [Test]
        public void ShouldMatchNothing()
        {
            var node = new FalseNode();
            
            Assert.False(node.Match(new RecentChange("", "", "", "", "", 0)));
            Assert.False(node.Match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
    }
}