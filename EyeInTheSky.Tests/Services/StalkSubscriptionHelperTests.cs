namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;

    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;

    using Moq;

    using NUnit.Framework;

    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    [TestFixture]
    public class StalkSubscriptionHelperTests : TestBase
    {
        private Mock<IIrcClient> ircClient;

        [SetUp]
        public void LocalSetup()
        {
            this.ircClient = new Mock<IIrcClient>();
            this.ircClient.Setup(x => x.ExtBanTypes).Returns(string.Empty);
        }

        [Test]
        public void SimpleSubscribe()
        {
            // arrange
            var stalk = new Mock<IStalk>();
            var channel = new Mock<IIrcChannel>();
            var subscriptionHelper = new StalkSubscriptionHelper(this.LoggerMock.Object);
            var mask = new IrcUserMask("*!*@*", this.ircClient.Object);

            channel.Setup(x => x.Identifier).Returns("#channel");
            stalk.Setup(x => x.Channel).Returns("#channel");

            var channelUserList = new List<ChannelUser>();
            channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            stalk.Setup(x => x.Subscribers).Returns(stalkUserList);
            
            // act
            SubscriptionSource source;
            var result = subscriptionHelper.SubscribeStalk(mask, channel.Object, stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
            Assert.AreEqual(true, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }
        
        [Test]
        public void SimpleUnsubscribe()
        {
            // arrange
            var stalk = new Mock<IStalk>();
            var channel = new Mock<IIrcChannel>();
            var subscriptionHelper = new StalkSubscriptionHelper(this.LoggerMock.Object);
            var mask = new IrcUserMask("*!*@*", this.ircClient.Object);

            channel.Setup(x => x.Identifier).Returns("#channel");
            stalk.Setup(x => x.Channel).Returns("#channel");

            var channelUserList = new List<ChannelUser>();
            channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>
            {
                new StalkUser(mask, true)
            };
            stalk.Setup(x => x.Subscribers).Returns(stalkUserList);
            
            // act
            SubscriptionSource source;
            var result = subscriptionHelper.UnsubscribeStalk(mask, channel.Object, stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, stalkUserList.Count);
            Assert.AreEqual(0, channelUserList.Count);
        }
        
        [Test]
        public void UnsubscribeWhenChannelSubscribed()
        {
            // arrange
            var stalk = new Mock<IStalk>();
            var channel = new Mock<IIrcChannel>();
            var subscriptionHelper = new StalkSubscriptionHelper(this.LoggerMock.Object);
            var mask = new IrcUserMask("*!*@*", this.ircClient.Object);

            channel.Setup(x => x.Identifier).Returns("#channel");
            stalk.Setup(x => x.Channel).Returns("#channel");

            var channelUserList = new List<ChannelUser>
            {
                new ChannelUser(mask)
            };
            channel.Setup(x => x.Users).Returns(channelUserList);

            var stalkUserList = new List<StalkUser>();
            stalk.Setup(x => x.Subscribers).Returns(stalkUserList);
            
            // act
            SubscriptionSource source;
            var result = subscriptionHelper.UnsubscribeStalk(mask, channel.Object, stalk.Object, out source);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, stalkUserList.Count);
            Assert.AreEqual(1, channelUserList.Count);
            Assert.AreEqual(false, stalkUserList[0].Subscribed);
            Assert.AreEqual("*!*@*", stalkUserList[0].Mask.ToString());
        }
    }
}