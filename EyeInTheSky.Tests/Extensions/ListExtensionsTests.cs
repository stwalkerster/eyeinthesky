namespace EyeInTheSky.Tests.Extensions
{
    using System.Collections.Generic;
    using EyeInTheSky.Extensions;
    using NUnit.Framework;

    /// <summary>
    /// The list extensions tests.
    /// </summary>
    [TestFixture]
    public class ListExtensionsTests
    {
        /// <summary>
        /// The pop from front test.
        /// </summary>
        [Test]
        public void PopFromFrontTest()
        {
            // arrange
            var data = new List<string> { "foo", "bar", "baz" };

            Assert.That(data.Count, Is.EqualTo(3));

            // act
            string popped = data.PopFromFront();

            // assert
            Assert.That(popped, Is.EqualTo("foo"));
            Assert.That(data.Count, Is.EqualTo(2));
        }
    }
}
