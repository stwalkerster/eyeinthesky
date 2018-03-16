namespace EyeInTheSky.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using EyeInTheSky.Helpers;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class StalkFactoryTests : TestBase
    {
        [Test, TestCaseSource(typeof(StalkFactoryTests), "DateParseTestCases")]
        public DateTime ShouldParseDateCorrectly(string inputDate)
        {
            var sf = new StalkFactory(this.LoggerMock.Object);
            
            return sf.ParseDate(string.Empty, inputDate, string.Empty);
        }

        public static IEnumerable<TestCaseData> DateParseTestCases
        {
            get
            {
                yield return new TestCaseData("03/16/2014 18:16:43")
                    .Returns(new DateTime(2014, 3, 16, 18, 16, 43));
                yield return new TestCaseData("12/31/9999 23:59:59")
                    .Returns(new DateTime(9999, 12, 31, 23, 59, 59));
                yield return new TestCaseData("01/01/1970 00:00:00")
                    .Returns(new DateTime(1970, 1, 1, 0, 0, 0));
                yield return new TestCaseData("2018-03-15T23:32:20.4640000+00:00")
                    .Returns(new DateTime(2018, 3, 15, 23, 32, 20, 464));
                yield return new TestCaseData("0001-01-01T00:00:00.0000000")
                    .Returns(new DateTime(1, 1, 1, 0, 0, 0, 0));
                yield return new TestCaseData("9999-12-31T23:59:59.9990000")
                    .Returns(new DateTime(9999, 12, 31, 23, 59, 59, 999));
            }
        }

        [Test]
        public void ShouldCreateBasicXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var ns = String.Empty;
            
            var node = new Mock<IStalkNode>();
            node.Setup(x => x.ToXmlFragment(doc, ns)).Returns(doc.CreateElement("false"));
            
            var stalk = new Mock<IStalk>();
            stalk.Setup(x => x.SearchTree).Returns(node.Object);
            stalk.Setup(x => x.Flag).Returns("testflag");
            stalk.Setup(x => x.IsEnabled).Returns(true);
            stalk.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new StalkFactory(this.LoggerMock.Object);

            // act
            var xmlElement = sf.ToXmlElement(stalk.Object, doc, "");
            
            // assert
            Assert.AreEqual("<complexstalk flag=\"testflag\" immediatemail=\"true\" enabled=\"true\"><false /></complexstalk>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateStalkXml()
        {
            // arrange
            var doc = new XmlDocument();
            var ns = String.Empty;
            
            doc.LoadXml("<or><true /><false /></or>");
            
            var node = new Mock<IStalkNode>();
            node.Setup(x => x.ToXmlFragment(doc, ns)).Returns(doc.DocumentElement);
            
            var stalk = new Mock<IStalk>();
            stalk.Setup(x => x.SearchTree).Returns(node.Object);
            stalk.Setup(x => x.Flag).Returns("testflag");
            stalk.Setup(x => x.IsEnabled).Returns(true);
            stalk.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new StalkFactory(this.LoggerMock.Object);

            // act
            var xmlElement = sf.ToXmlElement(stalk.Object, doc, "");
            
            // assert
            Assert.AreEqual("<complexstalk flag=\"testflag\" immediatemail=\"true\" enabled=\"true\"><or><true /><false /></or></complexstalk>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateCompleteXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var ns = String.Empty;
            
            var node = new Mock<IStalkNode>();
            node.Setup(x => x.ToXmlFragment(doc, ns)).Returns(doc.CreateElement("false"));
            
            var stalk = new Mock<IStalk>();
            stalk.Setup(x => x.SearchTree).Returns(node.Object);
            stalk.Setup(x => x.Flag).Returns("testflag");
            stalk.Setup(x => x.Description).Returns("my description here");
            stalk.Setup(x => x.IsEnabled).Returns(true);
            stalk.Setup(x => x.MailEnabled).Returns(true);
            stalk.Setup(x => x.LastUpdateTime).Returns(new DateTime(2018, 3, 14, 1, 2, 3));
            stalk.Setup(x => x.LastTriggerTime).Returns(DateTime.MinValue);
            stalk.Setup(x => x.ExpiryTime).Returns(DateTime.MaxValue);
            
            var sf = new StalkFactory(this.LoggerMock.Object);

            // act
            var xmlElement = sf.ToXmlElement(stalk.Object, doc, "");
            
            // assert
            Assert.AreEqual("<complexstalk flag=\"testflag\" lastupdate=\"2018-03-14T01:02:03Z\" lasttrigger=\"0001-01-01T00:00:00Z\" immediatemail=\"true\" description=\"my description here\" enabled=\"true\" expiry=\"9999-12-31T23:59:59.9999999Z\"><false /></complexstalk>", xmlElement.OuterXml);
        }
    }
}