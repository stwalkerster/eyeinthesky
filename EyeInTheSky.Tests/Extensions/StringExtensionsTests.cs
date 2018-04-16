namespace EyeInTheSky.Tests.Extensions
{
    using System.Linq;
    using EyeInTheSky.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ShouldHandleBasic()
        {
            var input = "foo bar baz";

            var result = StringExtensions.ToParameters(input).ToList();
            
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar", result[1]);
            Assert.AreEqual("baz", result[2]);
        }
        
        [Test]
        public void ShouldHandleDoubleSpace()
        {
            var input = "foo  bar baz";

            var result = StringExtensions.ToParameters(input).ToList();
            
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("", result[1]);
            Assert.AreEqual("bar", result[2]);
            Assert.AreEqual("baz", result[3]);
        }
        
        [Test]
        public void ShouldHandleEmpty()
        {
            var result = StringExtensions.ToParameters(string.Empty).ToList();
            
            Assert.AreEqual(0, result.Count);
        }
        
        [Test]
        public void ShouldHandleSingle()
        {
            var result = StringExtensions.ToParameters("foo").ToList();
            
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("foo", result[0]);
        }
        
        [Test]
        public void ShouldHandleSingleQuoted()
        {
            var result = StringExtensions.ToParameters("foo \"bar\" baz").ToList();
            
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar", result[1]);
            Assert.AreEqual("baz", result[2]);
        }
        
        [Test]
        public void ShouldHandleMultiQuoted()
        {
            var result = StringExtensions.ToParameters("foo \"bar baz\"").ToList();
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar baz", result[1]);
        }
        
        [Test]
        public void ShouldHandleQuotedDoubleSpace()
        {
            var result = StringExtensions.ToParameters("foo \"bar  baz\"").ToList();
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar  baz", result[1]);
        }
        
        [Test]
        public void ShouldHandleManyQuoted()
        {
            var result = StringExtensions.ToParameters("foo \"bar qux baz\"").ToList();
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar qux baz", result[1]);
        }
        
        [Test]
        public void ShouldHandleOpenQuote()
        {
            var result = StringExtensions.ToParameters("foo \"bar qux baz").ToList();
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("foo", result[0]);
            Assert.AreEqual("bar qux baz", result[1]);
        }
    }
}