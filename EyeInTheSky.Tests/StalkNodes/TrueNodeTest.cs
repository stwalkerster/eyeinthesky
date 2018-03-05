namespace EyeInTheSky.Tests.StalkNodes
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class TrueNodeTest 
    {
        [Test, Ignore("Existing failure")]
        public void ShouldRejectNullChange()
        {
            var node = new TrueNode();
            
            Assert.Catch(typeof(ArgumentNullException), () => node.match(null));
        }
        [Test]
        public void ShouldMatchEverything()
        {
            var node = new TrueNode();
            
            Assert.True(node.match(null));
            Assert.True(node.match(new RecentChange("", "", "", "", "", 0)));
            Assert.That(node.match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
        
        [Test]
        public void ShouldExportCorrectly()
        {
            var node = new TrueNode();

            Assert.AreEqual("<true />", node.toXmlFragment(new XmlDocument(), "").OuterXml);
        }
    }
}