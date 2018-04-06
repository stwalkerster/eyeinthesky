using System;

namespace EyeInTheSky.Tests.Model
{
    using EyeInTheSky.Model;
    using NUnit.Framework;

    [TestFixture]
    public class EmailConfigurationTests
    {
        private EmailConfiguration appConfig;

        [SetUp]
        public void LocalSetup()
        {
            this.appConfig = new EmailConfiguration("abc", "def", "ghi");
        }

        [Test]
        public void ShouldConstructCorrectly()
        {
            // assert
            Assert.AreEqual("abc", this.appConfig.Hostname);
            Assert.AreEqual("def", this.appConfig.Sender);
            Assert.AreEqual("ghi", this.appConfig.To);
        }

        [Test]
        public void ShouldThrowNullRefOnHostname()
        {
            Assert.Catch<ArgumentNullException>(() => { new EmailConfiguration(null, "def", "ghi"); });
        }

        [Test]
        public void ShouldThrowNullRefSender()
        {
            Assert.Catch<ArgumentNullException>(() => { new EmailConfiguration("abc", null, "ghi"); });
        }

        [Test]
        public void ShouldThrowNullRefTo()
        {
            Assert.Catch<ArgumentNullException>(() => { new EmailConfiguration("abc", "def", null); });
        }

        [Test]
        public void ShouldSetPort()
        {
            this.appConfig.Port = 1434;
            Assert.AreEqual(1434, this.appConfig.Port);
        }

        [Test]
        public void ShouldSetUsername()
        {
            this.appConfig.Username = "abc";
            Assert.AreEqual("abc", this.appConfig.Username);
        }

        [Test]
        public void ShouldSetPassword()
        {
            this.appConfig.Password = "abc";
            Assert.AreEqual("abc", this.appConfig.Password);
        }

        [Test]
        public void ShouldSetThumbprint()
        {
            this.appConfig.Thumbprint = "abc";
            Assert.AreEqual("abc", this.appConfig.Thumbprint);
        }
    }
}
