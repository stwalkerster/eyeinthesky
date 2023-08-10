using System;
using EyeInTheSky.Model.Interfaces;
using EyeInTheSky.Model.StalkNodes.BaseNodes;

namespace EyeInTheSky.Tests.Model.StalkNodes.BaseNodes
{
    using NUnit.Framework;

    [TestFixture]
    public class StalkNodeTests : StalkNodeTestBase<StalkNodeTests.NullNode>
    {
        [Test]
        public void ShouldThrowErrorOnForcedNullMatch()
        {
            var node = new NullNode();

            Assert.Throws<InvalidOperationException>(() => node.Match(this.RecentChangeBuilder()));
        }

        public class NullNode : StalkNode
        {
            protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
            {
                return null;
            }

            public override string ToString()
            {
                return "()";
            }
        }
    }
}