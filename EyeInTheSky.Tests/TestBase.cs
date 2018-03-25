namespace EyeInTheSky.Tests
{
    using System;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using Moq;
    using NUnit.Framework;

    public class TestBase
    {
        protected Mock<ILogger> LoggerMock { get; private set; }
        protected Mock<IAppConfiguration> AppConfigMock { get; private set; }
        protected Mock<INotificationTemplates> NotificationTemplatesMock { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = new Mock<ILogger>();
            this.AppConfigMock = new Mock<IAppConfiguration>();
            this.NotificationTemplatesMock = new Mock<INotificationTemplates>();

            this.AppConfigMock.Setup(s => s.DateFormat).Returns("u");
        }

        public class ValueProvider : DefaultValueProvider
        {
            protected override object GetDefaultValue(Type type, Mock mock)
            {
                if (type == typeof(string))
                {
                    return "abc";
                }
                
                if (type == typeof(int))
                {
                    return 123;
                }
                
                if (type == typeof(bool))
                {
                    return true;
                }

                return mock.Object;
            }
        }
    }
}