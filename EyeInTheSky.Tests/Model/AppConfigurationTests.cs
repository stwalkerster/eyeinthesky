﻿namespace EyeInTheSky.Tests.Model
{
    using System;
    using EyeInTheSky.Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppConfigurationTests
    {
        private AppConfiguration appConfig;

        [SetUp]
        public void LocalSetup()
        {
            this.appConfig = new AppConfiguration(
                "abc",
                "def",
                "ghi",
                "jkl",
                "efg",
                "mno",
                "p!q@r",
                "stu",
                "''d",
                "v!w@x");
        }

        [Test]
        public void ShouldConstructCorrectly()
        {
            // assert
            Assert.AreEqual("abc", this.appConfig.FreenodeChannel);
            Assert.AreEqual("def", this.appConfig.WikimediaChannel);
            Assert.AreEqual("ghi", this.appConfig.CommandPrefix);
            Assert.AreEqual("jkl", this.appConfig.UserConfigFile);
            Assert.AreEqual("efg", this.appConfig.ChannelConfigFile);
            Assert.AreEqual("mno", this.appConfig.TemplateConfigFile);
            Assert.AreEqual("p!q@r", this.appConfig.RcUser);
            Assert.AreEqual("stu", this.appConfig.DateFormat);
            Assert.AreEqual("''d", this.appConfig.TimeSpanFormat);
            Assert.AreEqual("v!w@x", this.appConfig.Owner);
        }

        [Test]
        public void ShouldSetEmailConfig()
        {
            var ec = Substitute.For<EmailConfiguration>("a", "b");

            this.appConfig.EmailConfiguration = ec;

            Assert.AreSame(ec, this.appConfig.EmailConfiguration);
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
                new AppConfiguration(null, "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefWikimedia()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", null, "ghi", "jkl", "efg", "mno", "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefCommandPrefix()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", null, "jkl", "efg", "mno", "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefStalkConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", null, "efg", "mno", "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefUserConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", null, "mno", "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefTemplateConf()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", null, "pqr", "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefRcUser()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", null, "stu", "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefDateFormat()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", null, "''d", "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefTimespanFormat()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", null, "vwx");
            });
        }

        [Test]
        public void ShouldThrowNullRefOwner()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new AppConfiguration("abc", "def", "ghi", "jkl", "efg", "mno", "pqr", "stu", "''d", null);
            });
        }
    }
}
