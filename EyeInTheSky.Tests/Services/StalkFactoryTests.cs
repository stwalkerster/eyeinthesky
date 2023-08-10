namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using NSubstitute;
    using NUnit.Framework;
    using Stwalkerster.IrcClient.Interfaces;

    [TestFixture]
    public class StalkFactoryTests : TestBase
    {
        [Test, TestCaseSource(typeof(StalkFactoryTests), nameof(DateParseTestCases))]
        public DateTime ShouldParseDateCorrectly(string inputDate, bool throwWarning)
        {
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            var sf = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);

            var actual = sf.ParseDate(string.Empty, inputDate, string.Empty);

            if (throwWarning)
            {
                this.LoggerMock.Received(1)
                    .WarnFormat(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }
            else
            {
                this.LoggerMock.Received(0)
                    .WarnFormat(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            }

            return actual;
        }

        public static IEnumerable<TestCaseData> DateParseTestCases
        {
            get
            {
                yield return new TestCaseData("03/16/2014 18:16:43", true)
                    .Returns(new DateTime(2014, 3, 16, 18, 16, 43));
                yield return new TestCaseData("12/31/9999 23:59:59", true)
                    .Returns(new DateTime(9999, 12, 31, 23, 59, 59));
                yield return new TestCaseData("01/01/1970 00:00:00", true)
                    .Returns(new DateTime(1970, 1, 1, 0, 0, 0));
                yield return new TestCaseData("2018-03-15T23:32:20.4640000+00:00", false)
                    .Returns(new DateTime(2018, 3, 15, 23, 32, 20, 464));
                yield return new TestCaseData("0001-01-01T00:00:00.0000000", false)
                    .Returns(new DateTime(1, 1, 1, 0, 0, 0, 0));
                yield return new TestCaseData("9999-12-31T23:59:59.9990000", false)
                    .Returns(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                yield return new TestCaseData("2018-03-15T23:32:20Z", false)
                    .Returns(new DateTime(2018, 3, 15, 23, 32, 20));
                yield return new TestCaseData("0001-01-01T00:00:00Z", false)
                    .Returns(new DateTime(1, 1, 1, 0, 0, 0, 0));
                yield return new TestCaseData("9999-12-31T23:59:59Z", false)
                    .Returns(new DateTime(9999, 12, 31, 23, 59, 59));
            }
        }

        [Test]
        public void ShouldCreateBasicXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var ns = string.Empty;
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.ToXml(doc, Arg.Any<IStalkNode>()).Returns(doc.CreateElement("false", ns));
            
            var node = Substitute.For<IStalkNode>();
            
            var stalk = Substitute.For<IStalk>();
            stalk.Description.Returns((string)null);
            stalk.LastMessageId.Returns((string)null);
            stalk.Subscribers.Returns(new List<StalkUser>());
            stalk.SearchTree.Returns(node);
            stalk.Identifier.Returns("testflag");
            stalk.IsEnabled.Returns(true);
            stalk.TriggerCount.Returns(4);
            stalk.WatchChannel.Returns("#en.wikipedia");
            
            
            var sf = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(stalk, doc);
            
            // assert
            var expected = "<complexstalk flag=\"testflag\" enabled=\"true\" watchchannel=\"#en.wikipedia\" triggercount=\"4\"><searchtree><false /></searchtree><subscribers /></complexstalk>";
            Assert.AreEqual(expected, xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateStalkXml()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            
            doc.LoadXml("<or><true /><false /></or>");
            
            var node = Substitute.For<IStalkNode>();
            snf.ToXml(doc, Arg.Any<IStalkNode>()).Returns(doc.DocumentElement);
            
            var stalk = Substitute.For<IStalk>();
            stalk.Description.Returns((string)null);
            stalk.LastMessageId.Returns((string)null);
            stalk.Subscribers.Returns(new List<StalkUser>());
            stalk.SearchTree.Returns(node);
            stalk.Identifier.Returns("testflag");
            stalk.IsEnabled.Returns(true);
            stalk.WatchChannel.Returns("#en.wikipedia");
            
            
            var sf = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(stalk, doc);
            
            // assert
            var expected = "<complexstalk flag=\"testflag\" enabled=\"true\" watchchannel=\"#en.wikipedia\" triggercount=\"0\"><searchtree><or><true /><false /></or></searchtree><subscribers /></complexstalk>";
            Assert.AreEqual(expected, xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateCompleteXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            
            var node = Substitute.For<IStalkNode>();
            snf.ToXml(doc, Arg.Any<IStalkNode>()).Returns(doc.CreateElement("false"));
           
            
            var stalk = Substitute.For<IStalk>();
            stalk.Subscribers.Returns(new List<StalkUser>());
            stalk.SearchTree.Returns(node);
            stalk.Identifier.Returns("testflag");
            stalk.Description.Returns("my description here");
            stalk.IsEnabled.Returns(true);
            stalk.LastUpdateTime.Returns(new DateTime(2018, 3, 14, 1, 2, 3));
            stalk.LastTriggerTime.Returns(DateTime.MinValue);
            stalk.ExpiryTime.Returns(DateTime.MaxValue);
            stalk.TriggerCount.Returns(3334);
            stalk.LastMessageId.Returns("foobar");
            stalk.WatchChannel.Returns("#metawiki");
            stalk.DynamicExpiry.Returns(new TimeSpan(90, 0, 0, 0));
            stalk.CreationDate.Returns(new DateTime(2019, 03, 28, 1, 2, 3));
            
            var sf = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(stalk, doc);
            
            // assert
            Assert.AreEqual("<complexstalk flag=\"testflag\" lastupdate=\"2018-03-14T01:02:03Z\" lasttrigger=\"0001-01-01T00:00:00Z\" creation=\"2019-03-28T01:02:03Z\" description=\"my description here\" lastmessageid=\"foobar\" enabled=\"true\" watchchannel=\"#metawiki\" expiry=\"9999-12-31T23:59:59.9999999Z\" dynamicexpiry=\"P90D\" triggercount=\"3334\"><searchtree><false /></searchtree><subscribers /></complexstalk>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateObjectFromLegacyXml()
        {
            string xml =
                "<complexstalk flag=\"testytest\" lastupdate=\"2018-03-25T16:42:30.984000Z\" lasttrigger=\"2018-03-25T16:42:21.878000Z\" immediatemail=\"true\" lastmessageid=\"foobar\" enabled=\"false\"><true /></complexstalk>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());
            
            // act
            var fact = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);
            var stalk = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(stalk.Description);
            Assert.IsNull(stalk.ExpiryTime);
            Assert.AreEqual("testytest", stalk.Identifier);
            Assert.IsFalse(stalk.IsEnabled);
            Assert.AreEqual("foobar", stalk.LastMessageId);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,21,878), stalk.LastTriggerTime);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), stalk.LastUpdateTime);
            Assert.AreEqual(0, stalk.TriggerCount);

            Assert.IsNotNull(stalk.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(stalk.SearchTree);

            snf.Received(1).NewFromXmlFragment(Arg.Any<XmlElement>());
        }

        [Test]
        public void ShouldCreateObjectFromXml()
        {
            string xml =
                "<complexstalk flag=\"testytest\" lastupdate=\"2018-03-25T16:42:30.984000Z\" lasttrigger=\"2018-03-25T16:42:21.878000Z\" creation=\"2019-01-01T23:23:23.090Z\" immediatemail=\"true\" lastmessageid=\"foobar\" enabled=\"false\"><searchtree><true /></searchtree></complexstalk>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());
            
            // act
            var fact = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);
            var stalk = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(stalk.Description);
            Assert.IsNull(stalk.ExpiryTime);
            Assert.AreEqual("testytest", stalk.Identifier);
            Assert.IsFalse(stalk.IsEnabled);
            Assert.AreEqual("#en.wikipedia", stalk.WatchChannel);
            Assert.AreEqual("foobar", stalk.LastMessageId);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,21,878), stalk.LastTriggerTime);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), stalk.LastUpdateTime);
            Assert.AreEqual(new DateTime(2019,01,01,23,23,23,90), stalk.CreationDate);
            Assert.AreEqual(0, stalk.TriggerCount);

            Assert.IsNotNull(stalk.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(stalk.SearchTree);
            
            snf.Received(1).NewFromXmlFragment(Arg.Any<XmlElement>());
        }
        
        [Test]
        public void ShouldCreateObjectFromXmlWithWatchChannel()
        {
            string xml =
                "<complexstalk flag=\"testytest\" lastupdate=\"2018-03-25T16:42:30.984000Z\" lasttrigger=\"2018-03-25T16:42:21.878000Z\" immediatemail=\"true\" lastmessageid=\"foobar\" enabled=\"false\" watchchannel=\"#fr.wikipedia\"><searchtree><true /></searchtree></complexstalk>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());
            
            // act
            var fact = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);
            var stalk = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(stalk.Description);
            Assert.IsNull(stalk.ExpiryTime);
            Assert.AreEqual("testytest", stalk.Identifier);
            Assert.IsFalse(stalk.IsEnabled);
            Assert.AreEqual("#fr.wikipedia", stalk.WatchChannel);
            Assert.AreEqual("foobar", stalk.LastMessageId);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,21,878), stalk.LastTriggerTime);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), stalk.LastUpdateTime);
            Assert.AreEqual(0, stalk.TriggerCount);

            Assert.IsNotNull(stalk.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(stalk.SearchTree);
            
            snf.Received(1).NewFromXmlFragment(Arg.Any<XmlElement>());
        }

        [Test]
        public void ShouldCreateObjectFromXmlWithTriggerCount()
        {
            string xml =
                "<complexstalk flag=\"testytest\" lastupdate=\"2018-03-25T16:42:30.984000Z\" lasttrigger=\"2018-03-25T16:42:21.878000Z\" immediatemail=\"true\" enabled=\"false\" triggercount=\"533\"><searchtree><true /></searchtree></complexstalk>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());
            
            // act
            var fact = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);
            var stalk = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(stalk.Description);
            Assert.IsNull(stalk.ExpiryTime);
            Assert.AreEqual("testytest", stalk.Identifier);
            Assert.IsFalse(stalk.IsEnabled);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,21,878), stalk.LastTriggerTime);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), stalk.LastUpdateTime);
            Assert.AreEqual(533, stalk.TriggerCount);
            
            Assert.IsNotNull(stalk.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(stalk.SearchTree);
            
            snf.Received(1).NewFromXmlFragment(Arg.Any<XmlElement>());
        }

        [Test]
        public void ShouldCreateObjectFromXmlWithExpiry()
        {
            string xml =
                "<complexstalk flag=\"testytest\" expiry=\"2018-03-25T16:42:30.984000Z\" dynamicexpiry=\"P3M\"><searchtree><true /></searchtree></complexstalk>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            var irc = Substitute.For<IIrcClient>();
            snf.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());

            // act
            var fact = new StalkFactory(this.LoggerMock, snf, irc, this.AppConfigMock);
            var stalk = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.AreEqual("testytest", stalk.Identifier);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), stalk.ExpiryTime);
            Assert.AreEqual(new TimeSpan(90, 0, 0, 0), stalk.DynamicExpiry);

            Assert.IsNotNull(stalk.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(stalk.SearchTree);

            snf.Received(1).NewFromXmlFragment(Arg.Any<XmlElement>());
        }
    }
}