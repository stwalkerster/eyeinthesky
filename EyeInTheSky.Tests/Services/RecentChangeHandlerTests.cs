namespace EyeInTheSky.Tests.Services
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Services;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class RecentChangeHandlerTests : TestBase
    {
        private Mock<IStalk> stalkMock;
        private Mock<IRecentChange> rcMock;

        [SetUp]
        public void LocalSetup()
        {
            this.Setup();

            this.stalkMock = new Mock<IStalk>();
            this.rcMock = new Mock<IRecentChange>();

            this.stalkMock.Setup(s => s.Flag).Returns("s1");
            this.stalkMock.Setup(s => s.Description).Returns("test desc");
            this.stalkMock.Setup(s => s.MailEnabled).Returns(true);
            this.stalkMock.Setup(s => s.ExpiryTime).Returns(DateTime.MaxValue);
            this.stalkMock.Setup(s => s.SearchTree).Returns(new TrueNode());

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
            s2.Setup(s => s.Flag).Returns("s2");

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
                this.NotificationTemplatesMock.Object);

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
            s2.Setup(s => s.Flag).Returns("s2");

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
                this.NotificationTemplatesMock.Object);

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
                this.NotificationTemplatesMock.Object);

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
                this.NotificationTemplatesMock.Object);

            // act
            var result = rcHander.FormatMessageForIrc(new[] {this.stalkMock.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual("[s1] url http://enwp.org page Foo by Me summ test end", result);
        }

        [Test]
        public void TestSingleFormatAllParamsForEmail()
        {
            // arrange
            this.NotificationTemplatesMock.Setup(s => s.EmailRcTemplate).Returns("{1} {2} {3} {4} {5} {6} | {0}");
            this.NotificationTemplatesMock.Setup(s => s.EmailStalkTemplate).Returns("> {0} {1} {2} {3} {4}");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object);

            // act
            var result = rcHander.FormatMessageForEmail(new[] {this.stalkMock.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual(
                "http://enwp.org Foo Me test +4 Minor | > s1 test desc (true) True 9999-12-31 23:59:59Z",
                result);
        }

        [Test]
        public void TestMultiFormatAllParamsForEmail()
        {
            // arrange
            this.NotificationTemplatesMock.Setup(s => s.EmailRcTemplate).Returns("{1} {2} {3} {4} {5} {6} | {0}");
            this.NotificationTemplatesMock.Setup(s => s.EmailStalkTemplate).Returns("> {0} {1} {2} {3} {4}");

            var rcHander = new RecentChangeHandler(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                null,
                null,
                null,
                null,
                this.NotificationTemplatesMock.Object);

            var s2 = new Mock<IStalk>();
            s2.Setup(s => s.Flag).Returns("s2");
            s2.Setup(s => s.Description).Returns("descky");
            s2.Setup(s => s.MailEnabled).Returns(false);
            s2.Setup(s => s.ExpiryTime).Returns(DateTime.MaxValue);
            s2.Setup(s => s.SearchTree).Returns(new FalseNode());

            // act
            var result = rcHander.FormatMessageForEmail(new[] {this.stalkMock.Object, s2.Object}, this.rcMock.Object);

            // assert
            Assert.AreEqual(
                "http://enwp.org Foo Me test +4 Minor | > s1 test desc (true) True 9999-12-31 23:59:59Z> s2 descky (false) False 9999-12-31 23:59:59Z",
                result);
        }
    }
}