namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TemplateFactoryTests : TestBase
    {
        [Test]
        public void ShouldCreateBasicXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            
            var snf = Substitute.For<IStalkNodeFactory>();
            
            var template = Substitute.For<ITemplate>();
            template.SearchTree.Returns("<false />");
            template.Description.Returns((string)null);
            template.StalkFlag.Returns((string)null);
            template.Identifier.Returns("testflag");
            template.StalkIsEnabled.Returns(true);
            template.TemplateIsEnabled.Returns(false);
            template.WatchChannel.Returns("#foo");
            
            
            var sf = new TemplateFactory(this.LoggerMock, snf, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(template, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" stalkenabled=\"true\" templateenabled=\"false\" watchchannel=\"#foo\"><searchtree><![CDATA[<false />]]></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateStalkXml()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = Substitute.For<IStalkNodeFactory>();
            
            var template = Substitute.For<ITemplate>();
            template.SearchTree.Returns("<or><true /><false /></or>");
            template.Description.Returns((string)null);
            template.StalkFlag.Returns((string)null);
            template.Identifier.Returns("testflag");
            template.StalkIsEnabled.Returns(true);
            template.TemplateIsEnabled.Returns(false);
            template.WatchChannel.Returns("#foo");
            
            var sf = new TemplateFactory(this.LoggerMock, snf, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(template, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" stalkenabled=\"true\" templateenabled=\"false\" watchchannel=\"#foo\"><searchtree><![CDATA[<or><true /><false /></or>]]></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateCompleteXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = Substitute.For<IStalkNodeFactory>();
           
            var template = Substitute.For<ITemplate>();
            template.SearchTree.Returns("<false />");
            template.Identifier.Returns("testflag");
            template.StalkFlag.Returns("bar");
            template.Description.Returns("my description here");
            template.StalkIsEnabled.Returns(true);
            template.TemplateIsEnabled.Returns(false);
            template.LastUpdateTime.Returns(new DateTime(2018, 3, 14, 1, 2, 3));
            template.ExpiryDuration.Returns(new TimeSpan(90, 0, 0, 0));
            template.WatchChannel.Returns("#bar");
            
            var sf = new TemplateFactory(this.LoggerMock, snf, this.AppConfigMock);

            // act
            var xmlElement = sf.ToXmlElement(template, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" lastupdate=\"2018-03-14T01:02:03Z\" description=\"my description here\" stalkflag=\"bar\" stalkenabled=\"true\" templateenabled=\"false\" watchchannel=\"#bar\" expiryduration=\"P90D\"><searchtree><![CDATA[<false />]]></searchtree></template>", xmlElement.OuterXml);
        }


        [Test]
        public void ShouldCreateObjectFromXml()
        {
            string xml =
                "<template flag=\"testytest\" stalkflag=\"bar\" lastupdate=\"2018-03-25T16:42:30.984000Z\" immediatemail=\"true\" templateenabled=\"true\" stalkenabled=\"false\" watchchannel=\"#quux\"><searchtree><![CDATA[<true />]]></searchtree></template>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = Substitute.For<IStalkNodeFactory>();
            
            // act
            var fact = new TemplateFactory(this.LoggerMock, snf, this.AppConfigMock);
            var template = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(template.Description);
            Assert.IsNull(template.ExpiryDuration);
            Assert.AreEqual("testytest", template.Identifier);
            Assert.AreEqual("bar", template.StalkFlag);
            Assert.IsFalse(template.StalkIsEnabled);
            Assert.IsTrue(template.TemplateIsEnabled);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), template.LastUpdateTime);
            Assert.AreEqual(template.SearchTree, "<true />");
            Assert.AreEqual("#quux", template.WatchChannel);
        }
    }
}