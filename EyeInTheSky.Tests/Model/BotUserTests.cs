namespace EyeInTheSky.Tests.Model
{
    using EyeInTheSky.Model;
    using Moq;
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class BotUserTests : TestBase
    {
        private IrcUserMask mask;

        [SetUp]
        public void LocalSetup()
        {
            var client = new Mock<IIrcClient>();
            client.Setup(x => x.ExtBanDelimiter).Returns("$");
            client.Setup(x => x.ExtBanTypes).Returns("ajrxz");

            this.mask = new IrcUserMask("$a:stwalkerster", client.Object);
        }

        [Test]
        public void TestNewUser()
        {
            var user = new BotUser(this.mask);
            
            Assert.AreEqual(this.mask, user.Mask);
            Assert.AreEqual("$a:stwalkerster", user.Identifier);
            
            Assert.IsFalse(user.EmailAddressConfirmed);
            Assert.IsNull(user.EmailAddress);
            Assert.IsNull(user.EmailConfirmationToken);
        }
        
        [Test]
        public void TestMailConfirmation()
        {
            var user = new BotUser(this.mask);
            
            Assert.AreEqual(this.mask, user.Mask);
            Assert.AreEqual("$a:stwalkerster", user.Identifier);
            
            Assert.IsFalse(user.EmailAddressConfirmed);
            Assert.IsNull(user.EmailAddress);
            Assert.IsNull(user.EmailConfirmationToken);

            user.EmailAddress = "abc";
            
            Assert.AreEqual("abc", user.EmailAddress);
            Assert.NotNull(user.EmailConfirmationToken);
            Assert.IsFalse(user.EmailAddressConfirmed);

            var firstToken = user.EmailConfirmationToken;
            
            user.ConfirmEmailAddress(user.EmailConfirmationToken);
            
            Assert.AreEqual("abc", user.EmailAddress);
            Assert.IsNull(user.EmailConfirmationToken);
            Assert.IsTrue(user.EmailAddressConfirmed);

            user.EmailAddress = "def";
            
            Assert.AreEqual("def", user.EmailAddress);
            Assert.NotNull(user.EmailConfirmationToken);
            Assert.IsFalse(user.EmailAddressConfirmed);
            
            Assert.AreNotEqual(firstToken, user.EmailConfirmationToken);
            Assert.GreaterOrEqual(user.EmailConfirmationToken.Length, 8);
        }
    }
}