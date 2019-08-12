namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;

    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Moq;

    using NUnit.Framework;

    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    [TestFixture]
    public class StalkSubscriptionHelperTests : TestBase
    {
        private Mock<IIrcClient> ircClient;
        private Mock<IStalk> stalk;
        private Mock<IIrcChannel> channel;
        private Mock<IBotUserConfiguration> botUserConfigMock;
        private StalkSubscriptionHelper subscriptionHelper;
        private IrcUserMask mask;

        [SetUp]
        public void LocalSetup()
        {
            this.ircClient = new Mock<IIrcClient>();
            this.ircClient.Setup(x => x.ExtBanTypes).Returns(string.Empty);

            this.channel = new Mock<IIrcChannel>();
            this.stalk = new Mock<IStalk>();
            this.botUserConfigMock = new Mock<IBotUserConfiguration>();

            this.channel.Setup(x => x.Identifier).Returns("#channel");
            this.stalk.Setup(x => x.Channel).Returns("#channel");

            this.subscriptionHelper = new StalkSubscriptionHelper(this.LoggerMock.Object, this.botUserConfigMock.Object);
            this.mask = new IrcUserMask("*!*@*", this.ircClient.Object);
        }

        [Test]
        public void SimpleSubscribe()
        {
            // arrange
            var channelUserList = new List<ChannelUser>();
            this.channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Setup(x => x.Subscribers).Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.SubscribeStalk(this.mask, this.channel.Object, this.stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
            Assert.AreEqual(true, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }

        [Test]
        public void SimpleSubscribeFail()
        {
            // arrange
            var channelUserList = new List<ChannelUser>();
            this.channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>
            {
                new StalkUser(this.mask, true)
            };
            this.stalk.Setup(x => x.Subscribers).Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.SubscribeStalk(this.mask, this.channel.Object, this.stalk.Object, out source);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
            Assert.AreEqual(true, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }

        [Test]
        public void SimpleUnsubscribe()
        {
            // arrange
            var channelUserList = new List<ChannelUser>();
            this.channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>
            {
                new StalkUser(this.mask, true)
            };
            this.stalk.Setup(x => x.Subscribers).Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel.Object, this.stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
        }

        [Test]
        public void SimpleUnsubscribeFail()
        {
            // arrange
            var channelUserList = new List<ChannelUser>();
            this.channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Setup(x => x.Subscribers).Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel.Object, this.stalk.Object, out source);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
        }

        [Test]
        public void UnsubscribeWhenChannelSubscribed()
        {
            // arrange
            var channelUserList = new List<ChannelUser>
            {
                new ChannelUser(this.mask) { Subscribed = true }
            };
            this.channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Setup(x => x.Subscribers).Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel.Object, this.stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(1, channelUserList.Count);
            Assert.AreEqual(false, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }
    }
}