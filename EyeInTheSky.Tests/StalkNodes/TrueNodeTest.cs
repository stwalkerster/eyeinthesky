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
        [Test]
        public void ShouldRejectNullChange()
        {
            var node = new TrueNode();
            
            Assert.Catch(typeof(ArgumentNullException), () => node.Match(null));
        }
        [Test]
        public void ShouldMatchEverything()
        {
            var node = new TrueNode();
            
            Assert.True(node.Match(new RecentChange("", "", "", "", "", 0)));
            Assert.That(node.Match(new RecentChange("a", "a", "a", "a", "a", 1)));
        }
        
        [Test]
        public void ShouldExportCorrectly()
        {
            var node = new TrueNode();

            Assert.AreEqual("<true />", node.ToXmlFragment(new XmlDocument(), "").OuterXml);
        }
    }
}