namespace EyeInTheSky.Tests.StalkNodes
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class FalseNodeTest 
    {
        [Test]
        public void ShouldRejectNullChange()
        {
            var node = new FalseNode();
            
            Assert.Catch(typeof(ArgumentNullException), () => node.Match(null));
        }
        
        [Test]
        public void ShouldMatchNothing()
        {
            var node = new FalseNode();
            
            Assert.False(node.Match(new RecentChange("", "", "", "", "", 0)));
            Assert.False(node.Match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
    }
}