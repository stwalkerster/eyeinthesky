namespace Stwalkerster.Extensions.Tests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void ShouldReturnEnumerableFromObject()
        {
            var obj = new object();

            var result = obj.ToEnumerable();
            
            Assert.True(typeof(IEnumerable<object>).IsInstanceOfType(result));
        }
        
        [Test]
        public void ShouldHaveObjectInEnumerable()
        {
            var obj = new object();

            var result = obj.ToEnumerable();
            
            Assert.AreEqual(obj, result.First());
        }
        
        [Test]
        public void ShouldHaveOneItemInEnumerable()
        {
            var obj = new object();

            var result = obj.ToEnumerable();
            
            Assert.AreEqual(obj, result.First());
        }
    }
}