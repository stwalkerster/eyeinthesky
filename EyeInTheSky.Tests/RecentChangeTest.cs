using NUnit.Framework;

namespace EyeInTheSky.Tests
{
    using EyeInTheSky.Model;

    [TestFixture]
    public class RecentChangeTest
    {
        [Test]
        public void ShouldConstructObjectCorrectly()
        {
            var foo = new RecentChange("a", "b", "c", "d", "e", 2);
            
            Assert.AreEqual("a", foo.Page);
            Assert.AreEqual("b", foo.User);
            Assert.AreEqual("c", foo.Url);
            Assert.AreEqual("d", foo.EditSummary);
            Assert.AreEqual("e", foo.EditFlags);
            Assert.AreEqual(2, foo.SizeDifference);
        }
    }
}