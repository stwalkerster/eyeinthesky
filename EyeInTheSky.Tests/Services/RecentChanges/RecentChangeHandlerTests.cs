namespace EyeInTheSky.Tests.Services.RecentChanges
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Services.RecentChanges;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using IrcChannel = EyeInTheSky.Model.IrcChannel;

    [TestFixture]
    public class RecentChangeHandlerTests : TestBase
    {
        private IStalk stalkMock;
        private IRecentChange rcMock;
        private IBotUser botUser;
        private IChannelConfiguration channelConfig;

        [SetUp]
        public void LocalSetup()
        {
            this.Setup();

            this.stalkMock = Substitute.For<IStalk>();
            this.rcMock = Substitute.For<IRecentChange>();
            this.botUser = Substitute.For<IBotUser>();
            this.channelConfig = Substitute.For<IChannelConfiguration>();

            var client = Substitute.For<IIrcClient>();
            client.ExtBanDelimiter.Returns("$");
            client.ExtBanTypes.Returns("a");
            var mask = new IrcUserMask("$a:abc", client);
            this.botUser.Mask.Returns(mask);

            this.channelConfig[Arg.Any<string>()].Returns(new IrcChannel("foo"));

            this.stalkMock.Identifier.Returns("s1");
            this.stalkMock.Description.Returns("test desc");
            this.stalkMock.ExpiryTime.Returns(DateTime.MaxValue);
            this.stalkMock.SearchTree.Returns(new TrueNode());
            this.stalkMock.Subscribers.Returns(new List<StalkUser>());

            this.rcMock.Url.Returns("http://enwp.org");
            this.rcMock.Page.Returns("Foo");
            this.rcMock.User.Returns("Me");
            this.rcMock.EditSummary.Returns("test");
            this.rcMock.SizeDiff.Returns(4);
            this.rcMock.EditFlags.Returns("Minor");
        }

        [Test]
        public void TestMultipleFormatStalkForIrc()
        {
            // arrange
            var s2 = Substitute.For<IStalk>();
            s2.Identifier.Returns("s2");

            this.NotificationTemplatesMock.IrcStalkTagSeparator.Returns("|");
            this.NotificationTemplatesMock.IrcAlertFormat.Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock,
                this.LoggerMock,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock,
                null,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock, s2}, this.rcMock);

            // assert
            Assert.AreEqual("[s1|s2] url http://enwp.org page Foo by Me summ test end", result);
        }

        [Test]
        public void TestMultipleCharFormatStalkForIrc()
        {
            // arrange
            var s2 = Substitute.For<IStalk>();
            s2.Identifier.Returns("s2");

            this.NotificationTemplatesMock.IrcStalkTagSeparator.Returns("<><>");
            this.NotificationTemplatesMock.IrcAlertFormat.Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock,
                this.LoggerMock,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock,
                null,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock, s2}, this.rcMock);

            // assert
            Assert.AreEqual("[s1<><>s2] url http://enwp.org page Foo by Me summ test end", result);
        }

        [Test]
        public void TestSingleFormatStalkAllParamsForIrc()
        {
            // arrange
            this.NotificationTemplatesMock.IrcStalkTagSeparator.Returns("|");
            this.NotificationTemplatesMock.IrcAlertFormat.Returns("[{0}] url {1} page {2} by {3} summ {4} size {5} flags {6} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock,
                this.LoggerMock,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock,
                null,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock}, this.rcMock);

            // assert
            Assert.AreEqual("[s1] url http://enwp.org page Foo by Me summ test size +4 flags Minor end", result);
        }

        [Test]
        public void TestSingleFormatFewerParamsForIrc()
        {
            // arrange
            this.NotificationTemplatesMock.IrcStalkTagSeparator.Returns("|");
            this.NotificationTemplatesMock.IrcAlertFormat.Returns("[{0}] url {1} page {2} by {3} summ {4} end");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock,
                this.LoggerMock,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock,
                null,
                null);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock}, this.rcMock);

            // assert
            Assert.AreEqual("[s1] url http://enwp.org page Foo by Me summ test end", result);
        }
    }
}