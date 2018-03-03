namespace EyeInTheSky.Tests.StalkNodes
{
    using System;
    using System.Xml;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class FalseNodeTest 
    {
        [Test, Ignore("Existing failure")]
        public void ShouldRejectNullChange()
        {
            var node = new FalseNode();
            
            Assert.Catch(typeof(ArgumentNullException), () => node.match(null));
        }
        
        [Test]
        public void ShouldMatchNothing()
        {
            var node = new FalseNode();
            
            Assert.False(node.match(new RecentChange("", "", "", "", "", 0)));
            Assert.False(node.match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
        
        [Test]
        public void ShouldExportCorrectly()
        {
            var node = new FalseNode();

            Assert.AreEqual("<false />", node.toXmlFragment(new XmlDocument(), "").OuterXml);
        }
    }
}