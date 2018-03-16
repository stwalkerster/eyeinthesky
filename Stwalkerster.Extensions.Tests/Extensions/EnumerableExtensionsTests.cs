namespace Stwalkerster.Extensions.Tests.Extensions
{
    using NUnit.Framework;

    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ShouldImplodeCorrectly()
        {
            var data = new[] {"foo", "bar", "baz"};

            var result = data.Implode();
            
            Assert.AreEqual("foo bar baz", result);
        }
        
        [Test]
        public void ShouldImplodeCorrectlyAltSep()
        {
            var data = new[] {"foo", "bar", "baz"};

            var result = data.Implode("|#");
            
            Assert.AreEqual("foo|#bar|#baz", result);
        }
        
        [Test]
        public void ShouldImplodeCorrectlySingleItem()
        {
            var data = new[] {"foo"};

            var result = data.Implode();
            
            Assert.AreEqual("foo", result);
        }
        
        [Test]
        public void ShouldImplodeCorrectlyEmpty()
        {
            var data = new string[0];

            var result = data.Implode();
            
            Assert.AreEqual(string.Empty, result);
        }
    }
}