namespace EyeInTheSky.Tests
{
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using NSubstitute;
    using NUnit.Framework;

    public class TestBase
    {
        protected ILogger LoggerMock { get; private set; }
        protected IAppConfiguration AppConfigMock { get; private set; }
        protected INotificationTemplates NotificationTemplatesMock { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = Substitute.For<ILogger>();
            this.AppConfigMock = Substitute.For<IAppConfiguration>();
            this.NotificationTemplatesMock = Substitute.For<INotificationTemplates>();

            this.AppConfigMock.DateFormat.Returns("u");
            this.AppConfigMock.WikimediaChannel.Returns("#en.wikipedia");
        }
    }
}