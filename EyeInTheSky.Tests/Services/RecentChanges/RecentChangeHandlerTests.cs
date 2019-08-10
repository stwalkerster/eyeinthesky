namespace EyeInTheSky.Tests.Services.RecentChanges
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges;
    using Moq;
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using IrcChannel = EyeInTheSky.Model.IrcChannel;

    [TestFixture]
    public class RecentChangeHandlerTests : TestBase
    {
        private Mock<IStalk> stalkMock;
        private Mock<IRecentChange> rcMock;
        private Mock<IBotUser> botUser;
        private Mock<IChannelConfiguration> channelConfig;

        [SetUp]
        public void LocalSetup()
        {
            this.Setup();

            this.stalkMock = new Mock<IStalk>();
            this.rcMock = new Mock<IRecentChange>();
            this.botUser = new Mock<IBotUser>();
            this.channelConfig = new Mock<IChannelConfiguration>();

            var client = new Mock<IIrcClient>();
            client.Setup(x => x.ExtBanDelimiter).Returns("$");
            client.Setup(x => x.ExtBanTypes).Returns("a");
            var mask = new IrcUserMask("$a:abc", client.Object);
            this.botUser.Setup(x => x.Mask).Returns(mask);

            this.channelConfig.Setup(x => x[It.IsAny<string>()]).Returns(new IrcChannel("foo"));

            this.stalkMock.Setup(s => s.Identifier).Returns("s1");
            this.stalkMock.Setup(s => s.Description).Returns("test desc");
            this.stalkMock.Setup(s => s.ExpiryTime).Returns(DateTime.MaxValue);
            this.stalkMock.Setup(s => s.SearchTree).Returns(new TrueNode());
            this.stalkMock.Setup(s => s.Subscribers).Returns(new List<StalkUser>());

            this.rcMock.Setup(s => s.Url).Returns("http://enwp.org");
            this.rcMock.Setup(s => s.Page).Returns("Foo");
            this.rcMock.Setup(s => s.User).Returns("Me");
            this.rcMock.Setup(s => s.EditSummary).Returns("test");
            this.rcMock.Setup(s => s.SizeDiff).Returns(4);
            this.rcMock.Setup(s => s.EditFlags).Returns("Minor");
        }

        [Test]
        public void TestMultipleFormatStalkForIrc()
        {
            // arrange
            var s2 = new Mock<IStalk>();
            s2.Setup(s => s.Identifier).Returns("s2");

            this.NotificationTemplatesMock.Setup(s => s.IrcStalkTagSeparator).Returns("|");
            this.NotificationTemplatesMock.Setup(s => s.IrcAlertFormat)
                .Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock.Object, s2.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual("[s1|s2] url http://enwp.org page Foo by Me summ test end", result);
        }

        [Test]
        public void TestMultipleCharFormatStalkForIrc()
        {
            // arrange
            var s2 = new Mock<IStalk>();
            s2.Setup(s => s.Identifier).Returns("s2");

            this.NotificationTemplatesMock.Setup(s => s.IrcStalkTagSeparator).Returns("<><>");
            this.NotificationTemplatesMock.Setup(s => s.IrcAlertFormat)
                .Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock.Object, s2.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual("[s1<><>s2] url http://enwp.org page Foo by Me summ test end", result);
        }

        [Test]
        public void TestSingleFormatStalkAllParamsForIrc()
        {
            // arrange
            this.NotificationTemplatesMock.Setup(s => s.IrcStalkTagSeparator).Returns("|");
            this.NotificationTemplatesMock.Setup(s => s.IrcAlertFormat)
                .Returns("[{0}] url {1} page {2} by {3} summ {4} size {5} flags {6} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual("[s1] url http://enwp.org page Foo by Me summ test size +4 flags Minor end", result);
        }

        [Test]
        public void TestSingleFormatFewerParamsForIrc()
        {
            // arrange
            this.NotificationTemplatesMock.Setup(s => s.IrcStalkTagSeparator).Returns("|");
            this.NotificationTemplatesMock.Setup(s => s.IrcAlertFormat)
                .Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual("[s1] url http://enwp.org page Foo by Me summ test end", result);
        }
    }
}