using System;
using EyeInTheSky.Model;

namespace EyeInTheSky.Tests.Model
{
    using NUnit.Framework;

    [TestFixture]
    public class ComplexStalkTests
    {
        [Test]
        public void ShouldConstructFlag()
        {
            var stalk = new ComplexStalk("abc");
            
            Assert.AreEqual("abc", stalk.Flag);
        }
        
        [Test]
        public void ShouldSetLastModified()
        {
            var stalk = new ComplexStalk("abc");

            var span = DateTime.Now - stalk.LastUpdateTime;
            
            Assert.True(span.HasValue);
            Assert.Less(span.Value.TotalSeconds, 5);
        }
        
        [Test]
        public void ShouldBeActive()
        {
            var stalk = new ComplexStalk("abc")
            {
                IsEnabled = true,
                ExpiryTime = null
            };

            Assert.IsTrue(stalk.IsActive());
        }
        
        [Test]
        public void ShouldBeActiveWithExpiry()
        {
            var stalk = new ComplexStalk("abc")
            {
                IsEnabled = true,
                ExpiryTime = DateTime.Now.AddMinutes(1)
            };

            Assert.IsTrue(stalk.IsActive());
        }
        
        [Test]
        public void ShouldNotBeActiveWithExpiry()
        {
            var stalk = new ComplexStalk("abc")
            {
                IsEnabled = true,
                ExpiryTime = DateTime.Now.AddMinutes(-1)
            };

            Assert.IsFalse(stalk.IsActive());
        }
        
        [Test]
        public void ShouldNotBeActive()
        {
            var stalk = new ComplexStalk("abc")
            {
                IsEnabled = false,
                ExpiryTime = null
            };

            Assert.IsFalse(stalk.IsActive());
        }
    }
}
