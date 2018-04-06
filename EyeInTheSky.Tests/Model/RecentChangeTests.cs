using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using EyeInTheSky.Model;
using EyeInTheSky.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace EyeInTheSky.Tests.Model
{
    class RecentChangeTests
    {

        [Test]
        public void ShouldGetUserInGroups()
        {
            var rc = new RecentChange("abc");
            var mwapi = new Mock<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi.Object;
            
            var expected = new[] {"*", "user", "autoconfirmed"};
            mwapi.Setup(x => x.GetUserGroups("abc")).Returns(expected);
            
            // act
            var result = rc.GetUserGroups();
            
            // assert
            Assert.AreEqual(expected, result);
            mwapi.Verify(x => x.GetUserGroups(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void ShouldGetIpGroups()
        {
            var rc = new RecentChange("1.2.3.4");
            var mwapi = new Mock<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi.Object;
            
            var expected = new[] {"*"};
            mwapi.Setup(x => x.GetUserGroups("1.2.3.4")).Returns(expected);
            
            // act
            var result = rc.GetUserGroups();
            
            // assert
            Assert.AreEqual(expected, result);
            mwapi.Verify(x => x.GetUserGroups(It.IsAny<string>()), Times.Once());
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
            var mwapi = new Mock<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi.Object;
            
            // act
            var pageIsInCategory = rc.PageIsInCategory("Bar");

            // assert
            Assert.IsFalse(pageIsInCategory);
            mwapi.Verify(x => x.PageIsInCategory(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShouldGetPageInCat()
        {
            var rc = new RecentChange("1.2.3.4") {Page = "Foo"};
            var mwapi = new Mock<IMediaWikiApi>();
            rc.MediaWikiApi = mwapi.Object;
            
            var expected = new[] {"*"};
            mwapi.Setup(x => x.PageIsInCategory("Foo", "Category:Bar")).Returns(true);
            
            // act
            var pageIsInCategory = rc.PageIsInCategory("Bar");

            // assert
            Assert.IsTrue(pageIsInCategory);
            mwapi.Verify(x => x.PageIsInCategory("Foo", "Category:Bar"), Times.Once);
        }
    }
}
