
namespace EyeInTheSky.Tests.Model
{
    using System;
    using EyeInTheSky.Model;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.Bot.MediaWikiLib.Services.Interfaces;

    class RecentChangeTests
    {

        [Test]
        public void ShouldGetUserInGroups()
        {
            var rc = new RecentChange("abc");
            var mwapi = Substitute.For<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi;
            
            var expected = new[] {"*", "user", "autoconfirmed"};
            mwapi.GetUserGroups("abc").Returns(expected);
            
            // act
            var result = rc.GetUserGroups();
            
            // assert
            Assert.AreEqual(expected, result);
            mwapi.Received(1).GetUserGroups(Arg.Any<string>());
        }

        [Test]
        public void ShouldGetIpGroups()
        {
            var rc = new RecentChange("1.2.3.4");
            var mwapi = Substitute.For<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi;
            
            var expected = new[] {"*"};
            mwapi.GetUserGroups("1.2.3.4").Returns(expected);
            
            // act
            var result = rc.GetUserGroups();
            
            // assert
            Assert.AreEqual(expected, result);
            mwapi.Received(1).GetUserGroups(Arg.Any<string>());
        }

        [Test]
        public void ShouldFailUsersWhenApiUnavailable()
        {
            var rc = new RecentChange("1.2.3.4");
           
            // act, assert
            Assert.Throws<InvalidOperationException>(() => rc.GetUserGroups());
        }
        
        [Test]
        public void ShouldFailCategoriesWhenApiUnavailable()
        {
            var rc = new RecentChange("1.2.3.4") {Page = "Foo"};
            
            // act, assert
            Assert.Throws<InvalidOperationException>(() => rc.PageIsInCategory("Bar"));
        }
        
        [Test]
        public void ShouldFailCategoriesWhenPageUnavailable()
        {
            var rc = new RecentChange("1.2.3.4");
            var mwapi = Substitute.For<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi;
            
            // act
            var pageIsInCategory = rc.PageIsInCategory("Bar");

            // assert
            Assert.IsFalse(pageIsInCategory);
            mwapi.Received(0).PageIsInCategory(Arg.Any<string>(),Arg.Any<string>());
        }

        [Test]
        public void ShouldGetPageInCat()
        {
            var rc = new RecentChange("1.2.3.4") {Page = "Foo"};
            var mwapi = Substitute.For<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi;
            
            mwapi.PageIsInCategory("Foo", "Category:Bar").Returns(true);
            
            // act
            var pageIsInCategory = rc.PageIsInCategory("Bar");

            // assert
            Assert.IsTrue(pageIsInCategory);
            mwapi.Received(1).PageIsInCategory("Foo", "Category:Bar");
        }
    }
}
