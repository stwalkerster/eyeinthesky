namespace Stwalkerster.IrcClient.Tests
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using ILogger = Castle.Core.Logging.ILogger;
    using Stwalkerster.IrcClient.Interfaces;

    /// <summary>
    /// The base class for all the unit tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Gets or sets the IRC configuration.
        /// </summary>
        protected Mock<IIrcConfiguration> IrcConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        protected Mock<ILogger> Logger { get; set; }
        
        /// <summary>
        /// The SupportHelper mock
        /// </summary>
        protected Mock<ISupportHelper> SupportHelper { get; set; }

        /// <summary>
        /// The common setup.
        /// </summary>
        [OneTimeSetUp]
        public void CommonSetup()
        {
            this.Logger = new Mock<ILogger>();
            this.Logger.Setup(x => x.CreateChildLogger(It.IsAny<string>())).Returns(this.Logger.Object);

            this.Logger.Setup(x => x.Fatal(It.IsAny<string>())).Callback(() => Assert.Fail("Logger recorded fatal error."));
            this.Logger.Setup(x => x.Fatal(It.IsAny<string>(), It.IsAny<Exception>())).Callback(() => Assert.Fail("Logger recorded fatal error."));
            this.Logger.Setup(x => x.FatalFormat(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<IEnumerable<object>>())).Callback(() => Assert.Fail("Logger recorded fatal error."));
            this.Logger.Setup(x => x.FatalFormat(It.IsAny<string>(), It.IsAny<IEnumerable<object>>())).Callback(() => Assert.Fail("Logger recorded fatal error.")); 
            
            this.Logger.Setup(x => x.Error(It.IsAny<string>())).Callback(() => Assert.Fail("Logger recorded error."));
            this.Logger.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>())).Callback(() => Assert.Fail("Logger recorded error."));
            this.Logger.Setup(x => x.ErrorFormat(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<IEnumerable<object>>())).Callback(() => Assert.Fail("Logger recorded error."));
            this.Logger.Setup(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<IEnumerable<object>>())).Callback(() => Assert.Fail("Logger recorded error."));

            this.IrcConfiguration = new Mock<IIrcConfiguration>();
            
            this.SupportHelper = new Mock<ISupportHelper>();

            this.LocalSetup();
        }

        /// <summary>
        /// The local setup.
        /// </summary>
        public virtual void LocalSetup()
        {
        }
    }
}
