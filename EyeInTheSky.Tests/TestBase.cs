﻿namespace EyeInTheSky.Tests
{
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using Moq;
    using NUnit.Framework;

    public class TestBase
    {
        public Mock<ILogger> LoggerMock { get; private set; }
        public Mock<IAppConfiguration> AppConfigMock { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.LoggerMock = new Mock<ILogger>();
            this.AppConfigMock = new Mock<IAppConfiguration>();
        }
    }
}