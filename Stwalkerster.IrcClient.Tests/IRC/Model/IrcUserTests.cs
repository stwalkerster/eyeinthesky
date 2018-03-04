namespace Stwalkerster.IrcClient.Tests.IRC.Model
{
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Model;

    /// <summary>
    /// The IRC user tests.
    /// </summary>
    [TestFixture]
    public class IrcUserTests : TestBase
    {
        /// <summary>
        /// The should create from prefix.
        /// </summary>
        [Test]
        public void ShouldCreateFromPrefix()
        {
            // arrange
            const string Prefix = "Yetanotherx|afk!~Yetanothe@mcbouncer.com";
            var expected = new IrcUser
                               {
                                   Hostname = "mcbouncer.com",
                                   Username = "~Yetanothe",
                                   Nickname = "Yetanotherx|afk"
                               };

            // act
            var actual = IrcUser.FromPrefix(Prefix);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// The should create from prefix 2.
        /// </summary>
        [Test]
        public void ShouldCreateFromPrefix2()
        {
            // arrange
            const string Prefix = "stwalkerster@foo.com";
            var expected = new IrcUser
            {
                Hostname = "foo.com",
                Nickname = "stwalkerster"
            };

            // act
            var actual = IrcUser.FromPrefix(Prefix);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// The should create from prefix 3.
        /// </summary>
        [Test]
        public void ShouldCreateFromPrefix3()
        {
            // arrange
            const string Prefix = "stwalkerster";
            var expected = new IrcUser
            {
                Nickname = "stwalkerster"
            };

            // act
            var actual = IrcUser.FromPrefix(Prefix);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// The should create from prefix.
        /// </summary>
        [Test]
        public void ShouldCreateFromPrefix4()
        {
            // arrange
            const string Prefix = "nick!user@host";
            var expected = new IrcUser
            {
                Hostname = "host",
                Username = "user",
                Nickname = "nick"
            };

            // act
            var actual = IrcUser.FromPrefix(Prefix);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// The should create from prefix.
        /// </summary>
        [Test]
        public void ShouldCreateFromPrefix5()
        {
            // arrange
            const string Prefix = "ChanServ!ChanServ@services.";
            var expected = new IrcUser
            {
                Hostname = "services.",
                Username = "ChanServ",
                Nickname = "ChanServ"
            };

            // act
            var actual = IrcUser.FromPrefix(Prefix);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
