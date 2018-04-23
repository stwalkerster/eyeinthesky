namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TemplateFactoryTests : TestBase
    {
        [Test]
        public void ShouldCreateBasicXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            
            var snf = new Mock<IStalkNodeFactory>();
            
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns("<false />");
            template.Setup(x => x.Flag).Returns("testflag");
            template.Setup(x => x.StalkIsEnabled).Returns(true);
            template.Setup(x => x.TemplateIsEnabled).Returns(false);
            template.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new TemplateFactory(this.LoggerMock.Object, snf.Object);

            // act
            var xmlElement = sf.ToXmlElement(template.Object, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\"><searchtree><![CDATA[<false />]]></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateStalkXml()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = new Mock<IStalkNodeFactory>();
            
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns("<or><true /><false /></or>");
            template.Setup(x => x.Flag).Returns("testflag");
            template.Setup(x => x.StalkIsEnabled).Returns(true);
            template.Setup(x => x.TemplateIsEnabled).Returns(false);
            template.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new TemplateFactory(this.LoggerMock.Object, snf.Object);

            // act
            var xmlElement = sf.ToXmlElement(template.Object, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\"><searchtree><![CDATA[<or><true /><false /></or>]]></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateCompleteXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = new Mock<IStalkNodeFactory>();
           
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns("<false />");
            template.Setup(x => x.Flag).Returns("testflag");
            template.Setup(x => x.StalkFlag).Returns("bar");
            template.Setup(x => x.Description).Returns("my description here");
            template.Setup(x => x.StalkIsEnabled).Returns(true);
            template.Setup(x => x.TemplateIsEnabled).Returns(false);
            template.Setup(x => x.MailEnabled).Returns(true);
            template.Setup(x => x.LastUpdateTime).Returns(new DateTime(2018, 3, 14, 1, 2, 3));
            template.Setup(x => x.ExpiryDuration).Returns(new TimeSpan(90, 0, 0, 0));
            
            var sf = new TemplateFactory(this.LoggerMock.Object, snf.Object);

            // act
            var xmlElement = sf.ToXmlElement(template.Object, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" lastupdate=\"2018-03-14T01:02:03Z\" description=\"my description here\" stalkflag=\"bar\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\" expiryduration=\"P90D\"><searchtree><![CDATA[<false />]]></searchtree></template>", xmlElement.OuterXml);
        }


        [Test]
        public void ShouldCreateObjectFromXml()
        {
            string xml =
                "<template flag=\"testytest\" stalkflag=\"bar\" lastupdate=\"2018-03-25T16:42:30.984000Z\" immediatemail=\"true\" templateenabled=\"true\" stalkenabled=\"false\"><searchtree><![CDATA[<true />]]></searchtree></template>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = new Mock<IStalkNodeFactory>();
            
            // act
            var fact = new TemplateFactory(this.LoggerMock.Object, snf.Object);
            var template = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(template.Description);
            Assert.IsNull(template.ExpiryDuration);
            Assert.AreEqual("testytest", template.Flag);
            Assert.AreEqual("bar", template.StalkFlag);
            Assert.IsFalse(template.StalkIsEnabled);
            Assert.IsTrue(template.TemplateIsEnabled);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), template.LastUpdateTime);
            Assert.AreEqual(template.SearchTree, "<true />");
        }
    }
}