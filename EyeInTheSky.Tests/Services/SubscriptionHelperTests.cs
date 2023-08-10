namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    [TestFixture]
    public class SubscriptionHelperTests : TestBase
    {
        private IIrcClient ircClient;
        private IStalk stalk;
        private IIrcChannel channel;
        private IBotUserConfiguration botUserConfigMock;
        private SubscriptionHelper subscriptionHelper;
        private IrcUserMask mask;
        private IChannelConfiguration channelConfigMock;

        [SetUp]
        public void LocalSetup()
        {
            this.ircClient = Substitute.For<IIrcClient>();
            this.ircClient.ExtBanTypes.Returns(string.Empty);

            this.channel = Substitute.For<IIrcChannel>();
            this.stalk = Substitute.For<IStalk>();
            this.botUserConfigMock = Substitute.For<IBotUserConfiguration>();
            this.channelConfigMock = Substitute.For<IChannelConfiguration>();

            this.channel.Identifier.Returns("#channel");
            this.stalk.Channel.Returns("#channel");

            this.subscriptionHelper = new SubscriptionHelper(this.LoggerMock, this.botUserConfigMock, this.channelConfigMock);
            this.mask = new IrcUserMask("*!*@*", this.ircClient);
        }

        [Test]
        public void SimpleSubscribe()
        {
            // arrange
            var channelUserList = new List<ChannelUser>();
            this.channel.Users.Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Subscribers.Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.SubscribeStalk(this.mask, this.channel, this.stalk, out source);

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
            this.channel.Users.Returns(channelUserList);

            var stalkUserList = new List<StalkUser>
            {
                new StalkUser(this.mask, true)
            };
            this.stalk.Subscribers.Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.SubscribeStalk(this.mask, this.channel, this.stalk, out source);

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
            this.channel.Users.Returns(channelUserList);

            var stalkUserList = new List<StalkUser>
            {
                new StalkUser(this.mask, true)
            };
            this.stalk.Subscribers.Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel, this.stalk, out source);

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
            this.channel.Users.Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Subscribers.Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel, this.stalk, out source);

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
            this.channel.Users.Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            this.stalk.Subscribers.Returns(stalkUserList);

            // act
            SubscriptionSource source;
            var result = this.subscriptionHelper.UnsubscribeStalk(this.mask, this.channel, this.stalk, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(1, channelUserList.Count);
            Assert.AreEqual(false, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }
    }
}