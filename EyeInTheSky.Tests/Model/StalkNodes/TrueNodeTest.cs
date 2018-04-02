namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NUnit.Framework;

    [TestFixture]
    public class TrueNodeTest : LogicalNodeTestBase<TrueNode>
    {
        [Test]
        public void ShouldMatchEverything()
        {
            var node = new TrueNode();

            Assert.True(node.Match(new RecentChange("")));
            Assert.That(node.Match(this.RecentChangeBuilder().Object));
        }
    }
}