namespace EyeInTheSky.Tests.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class TrueNodeTest : LogicalNodeTestBase<TrueNode>
    {
        [Test]
        public void ShouldMatchEverything()
        {
            var node = new TrueNode();
            
            Assert.True(node.Match(new RecentChange("", "", "", "", "", 0)));
            Assert.That(node.Match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
    }
}