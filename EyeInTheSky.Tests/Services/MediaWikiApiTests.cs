﻿namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using EyeInTheSky.Services;
    using EyeInTheSky.Services.Interfaces;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class MediaWikiApiTests : TestBase
    {
        private Mock<IWebServiceClient> wsClient;
        private MediaWikiApi mwApi;

        [SetUp]
        public void LocalSetup()
        {
            this.wsClient = new Mock<IWebServiceClient>();
            this.mwApi = new MediaWikiApi(this.LoggerMock.Object, this.wsClient.Object);
        }
        
        [Test, TestCaseSource(typeof(MediaWikiApiTests), "GroupParseTestCases")]
        public List<string> ShouldParseGroupsCorrectly(string user, string input)
        {
            // arrange
            var memstream = new MemoryStream();
            var sw = new StreamWriter(memstream);
            sw.Write(input);
            sw.Flush();
            memstream.Position = 0;
            
            this.wsClient.Setup(x => x.DoApiCall(It.IsAny<NameValueCollection>())).Returns(memstream);
            
            // act
            return this.mwApi.GetUserGroups(user).ToList();
        }

        public static IEnumerable<TestCaseData> GroupParseTestCases
        {
            get
            {
                yield return new TestCaseData(
                        "Stwalkerster",
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user userid=\"851859\" name=\"Stwalkerster\"><groups><g>abusefilter</g><g>sysop</g><g>*</g><g>user</g><g>autoconfirmed</g></groups></user></users></query></api>")
                    .Returns(new List<string> {"abusefilter", "sysop", "*", "user", "autoconfirmed"});
                yield return new TestCaseData(
                        "Stwnonexist",
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user name=\"Stwnonexist\" missing=\"\" /></users></query></api>")
                    .Returns(new List<string> {"*"});
                yield return new TestCaseData(
                        "127.0.0.1",
                        "<?xml version=\"1.0\"?><api batchcomplete=\"\"><query><users><user name=\"127.0.0.1\" invalid=\"\" /></users></query></api>")
                    .Returns(new List<string> {"*"});
            }
        }
    }
}