namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NUnit.Framework;

    [TestFixture]
    public class FalseNodeTest : LogicalNodeTestBase<FalseNode>
    {
        [Test]
        public void ShouldMatchNothing()
        {
            var node = new FalseNode();

            Assert.False(node.Match(new RecentChange("")));
            Assert.False(node.Match(this.RecentChangeBuilder().Object));
        }
    }
}