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
    using NSubstitute;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using NUnit.Framework;

    [TestFixture]
    public class TemplateConfigurationTests : TestBase
    {
        private ITemplateFactory templateFact;
        private ICommandParser commandParser;
        private IStalkNodeFactory stalkNodeFact;
        private IFileService fileService;

        [SetUp]
        public void LocalSetup()
        {
            this.templateFact = Substitute.For<ITemplateFactory>();
            this.commandParser = Substitute.For<ICommandParser>();
            this.stalkNodeFact = Substitute.For<IStalkNodeFactory>();
            this.fileService = Substitute.For<IFileService>();
            
            this.fileService.FileExists(Arg.Any<string>()).Returns(true);
            this.fileService.GetWritableStream(Arg.Any<string>()).Returns(new MemoryStream());
            
            this.templateFact.ToXmlElement(Arg.Any<ITemplate>(), Arg.Any<XmlDocument>())
                .Returns((ci) => ci.Arg<XmlDocument>().CreateElement("template"));
        }
        
        [Test]
        public void ShouldConvertTemplateToStalk()
        {
            var data = "<eyeinthesky><templates><template /></templates></eyeinthesky>";

            this.fileService.GetReadableStream(Arg.Any<string>())
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes(data)));

            var tpl = new Template("flag", null, true, true, null, DateTime.MinValue, null, "<true />", "#meta");
            this.templateFact.NewFromXmlElement(Arg.Any<XmlElement>()).Returns(tpl);
            this.stalkNodeFact.NewFromXmlFragment(Arg.Any<XmlElement>()).Returns(new TrueNode());
            
            var templateConfig = new TemplateConfiguration(
                this.AppConfigMock,
                this.LoggerMock,
                this.templateFact,
                this.commandParser,
                this.stalkNodeFact,
                this.fileService
            );
            templateConfig.Initialize();
            
            // act  
            var result = templateConfig.NewFromTemplate("stalk", tpl, new List<string>());
            
            // assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<IStalk>(result);
            Assert.AreEqual("stalk", result.Identifier);
            Assert.IsInstanceOf<TrueNode>(result.SearchTree);
            Assert.AreEqual("#meta", result.WatchChannel);
        }
    }
}