namespace EyeInTheSky.Tests
{
    using EyeInTheSky.Model;
    using NUnit.Framework;
    
    [TestFixture]
    public class RecentChangeTest
    {
        [Test]
        public void ShouldConstructObjectCorrectly()
        {
            var foo = new RecentChange("def")
            {
                Page = "abc",
                Url = "ghi",
                EditSummary = "jkl",
                EditFlags = "mno",
                SizeDiff = 123,
                Log = "pqr"
            };

            Assert.AreEqual("abc", foo.Page);
            Assert.AreEqual("def", foo.User);
            Assert.AreEqual("ghi", foo.Url);
            Assert.AreEqual("jkl", foo.EditSummary);
            Assert.AreEqual("mno", foo.EditFlags);
            Assert.AreEqual(123, foo.SizeDiff);
            Assert.AreEqual("pqr", foo.Log);
        }
    }
}