namespace EyeInTheSky.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Moq;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using NUnit.Framework;

    [TestFixture]
    public class TemplateConfigurationTests : TestBase
    {
        private Mock<ITemplateFactory> templateFact;
        private Mock<ICommandParser> commandParser;
        private Mock<IStalkNodeFactory> stalkNodeFact;
        private Mock<IFileService> fileService;

        [SetUp]
        public void LocalSetup()
        {
            this.templateFact = new Mock<ITemplateFactory>();
            this.commandParser = new Mock<ICommandParser>();
            this.stalkNodeFact = new Mock<IStalkNodeFactory>();
            this.fileService = new Mock<IFileService>();
            
            this.fileService.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            this.fileService.Setup(x => x.GetWritableStream(It.IsAny<string>())).Returns(new MemoryStream());
            
            this.templateFact.Setup(x => x.ToXmlElement(It.IsAny<ITemplate>(), It.IsAny<XmlDocument>()))
                .Returns<ITemplate, XmlDocument>((t, d) => d.CreateElement("template"));
        }
        
        [Test]
        public void ShouldConvertTemplateToStalk()
        {
            var data = "<eyeinthesky><templates><template /></templates></eyeinthesky>";

            this.fileService.Setup(x => x.GetReadableStream(It.IsAny<string>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var tpl = new Template("flag", null, true, true, true, null, DateTime.MinValue, null, "<true />");
            this.templateFact.Setup(x => x.NewFromXmlElement(It.IsAny<XmlElement>())).Returns(tpl);
            this.stalkNodeFact.Setup(x => x.NewFromXmlFragment(It.IsAny<XmlElement>())).Returns(new TrueNode());
            
            var templateConfig = new TemplateConfiguration(
                this.AppConfigMock.Object,
                this.LoggerMock.Object,
                this.templateFact.Object,
                this.commandParser.Object,
                this.stalkNodeFact.Object,
                this.fileService.Object
            );
            templateConfig.Initialize();
            
            // act  
            var result = templateConfig.NewFromTemplate("stalk", tpl, new List<string>());
            
            // assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<IStalk>(result);
            Assert.AreEqual("stalk", result.Flag);
            Assert.IsInstanceOf<TrueNode>(result.SearchTree);
        }
    }
}