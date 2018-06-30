﻿namespace EyeInTheSky.Tests.Model
{
    using System;
    using EyeInTheSky.Model;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AppConfigurationTests
    {
        private AppConfiguration appConfig;

        [SetUp]
        public void LocalSetup()
        {
            this.appConfig = new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "p!q@r", "stu", "v!w@x", "yza", "bcd");
        }

        [Test]
        public void ShouldConstructCorrectly()
        {
            // assert
            Assert.AreEqual("abc", this.appConfig.FreenodeChannel);
            Assert.AreEqual("def", this.appConfig.WikimediaChannel);
            Assert.AreEqual("ghi", this.appConfig.CommandPrefix);
            Assert.AreEqual("jkl", this.appConfig.StalkConfigFile);
            Assert.AreEqual("efg", this.appConfig.UserConfigFile);
            Assert.AreEqual("mno", this.appConfig.TemplateConfigFile);
            Assert.AreEqual("p!q@r", this.appConfig.RcUser);
            Assert.AreEqual("stu", this.appConfig.DateFormat);
            Assert.AreEqual("v!w@x", this.appConfig.Owner);
            Assert.AreEqual("yza", this.appConfig.MediaWikiApiEndpoint);
            Assert.AreEqual("bcd", this.appConfig.UserAgent);
        }

        [Test]
        public void ShouldSetEmailConfig()
        {
            var ec = new Mock<EmailConfiguration>("a", "b", "c");

            this.appConfig.EmailConfiguration = ec.Object;

            Assert.AreSame(ec.Object, this.appConfig.EmailConfiguration);
        }

        [Test]
        public void ShouldSetMonitorPort()
        {
            this.appConfig.MonitoringPort = 1434;
            Assert.AreEqual(1434, this.appConfig.MonitoringPort);
        }

        [Test]
        public void ShouldThrowNullRefOnFreenode()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration(null, "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefWikimedia()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", null, "ghi", "jkl", "efg", "mno", "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefCommandPrefix()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", null, "jkl", "efg", "mno", "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefStalkConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", null, "efg", "mno", "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefUserConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", null, "mno", "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefTemplateConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", null, "pqr", "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefRcUser()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", null, "stu", "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefDateFormat()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", null, "vwx", "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefOwner()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", null, "yza", "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefMWEndpoint()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", "vwx", null, "bcd");
            });
        }

        [Test]
        public void ShouldThrowNullRefUserAgent()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", "vwx", "yza", null);
            });
        }
    }
}
