namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
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
            var ns = string.Empty;
            var snf = new Mock<IStalkNodeFactory>();
            snf.Setup(x => x.ToXml(doc, It.IsAny<IStalkNode>())).Returns(doc.CreateElement("false", ns));
            
            var node = new Mock<IStalkNode>();
            
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns(node.Object);
            template.Setup(x => x.Flag).Returns("testflag");
            template.Setup(x => x.StalkIsEnabled).Returns(true);
            template.Setup(x => x.TemplateIsEnabled).Returns(false);
            template.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new TemplateFactory(this.LoggerMock.Object, snf.Object);

            // act
            var xmlElement = sf.ToXmlElement(template.Object, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\"><searchtree><false /></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateStalkXml()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = new Mock<IStalkNodeFactory>();
            
            doc.LoadXml("<or><true /><false /></or>");
            
            var node = new Mock<IStalkNode>();
            snf.Setup(x => x.ToXml(doc, It.IsAny<IStalkNode>())).Returns(doc.DocumentElement);
            
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns(node.Object);
            template.Setup(x => x.Flag).Returns("testflag");
            template.Setup(x => x.StalkIsEnabled).Returns(true);
            template.Setup(x => x.TemplateIsEnabled).Returns(false);
            template.Setup(x => x.MailEnabled).Returns(true);
            
            
            var sf = new TemplateFactory(this.LoggerMock.Object, snf.Object);

            // act
            var xmlElement = sf.ToXmlElement(template.Object, doc);
            
            // assert
            Assert.AreEqual("<template flag=\"testflag\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\"><searchtree><or><true /><false /></or></searchtree></template>", xmlElement.OuterXml);
        }

        [Test]
        public void ShouldCreateCompleteXmlElement()
        {
            // arrange
            var doc = new XmlDocument();
            var snf = new Mock<IStalkNodeFactory>();
            
            var node = new Mock<IStalkNode>();
            snf.Setup(x => x.ToXml(doc, It.IsAny<IStalkNode>())).Returns(doc.CreateElement("false"));
           
            
            var template = new Mock<ITemplate>();
            template.Setup(x => x.SearchTree).Returns(node.Object);
            template.Setup(x => x.Flag).Returns("testflag");
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
            Assert.AreEqual("<template flag=\"testflag\" lastupdate=\"2018-03-14T01:02:03Z\" description=\"my description here\" immediatemail=\"true\" stalkenabled=\"true\" templateenabled=\"false\" expiryduration=\"P90D\"><searchtree><false /></searchtree></template>", xmlElement.OuterXml);
        }


        [Test]
        public void ShouldCreateObjectFromXml()
        {
            string xml =
                "<template flag=\"testytest\" lastupdate=\"2018-03-25T16:42:30.984000Z\" immediatemail=\"true\" templateenabled=\"true\" stalkenabled=\"false\"><searchtree><true /></searchtree></template>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var snf = new Mock<IStalkNodeFactory>();
            snf.Setup(x => x.NewFromXmlFragment(It.IsAny<XmlElement>())).Returns(new TrueNode());
            
            // act
            var fact = new TemplateFactory(this.LoggerMock.Object, snf.Object);
            var template = fact.NewFromXmlElement(doc.DocumentElement);

            // assert
            Assert.IsNull(template.Description);
            Assert.IsNull(template.ExpiryDuration);
            Assert.AreEqual("testytest", template.Flag);
            Assert.IsFalse(template.StalkIsEnabled);
            Assert.IsTrue(template.TemplateIsEnabled);
            Assert.AreEqual(new DateTime(2018,03,25,16,42,30,984), template.LastUpdateTime);

            Assert.IsNotNull(template.SearchTree);
            Assert.IsInstanceOf<IStalkNode>(template.SearchTree);
            
            snf.Verify(x => x.NewFromXmlFragment(It.IsAny<XmlElement>()), Times.Once);
        }
    }
}