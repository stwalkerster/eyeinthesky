namespace EyeInTheSky.Tests.Model
{
    using System;
    using EyeInTheSky.Model;
    using NUnit.Framework;
    
    [TestFixture]
    class NotificationTemplateTests
    {
        [Test]
        public void ShouldConstructCorrectly()
        {
            var nt = new NotificationTemplates("abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yza");

            // assert
            Assert.AreEqual("abc", nt.EmailRcSubject);
            Assert.AreEqual("def", nt.EmailRcTemplate);
            Assert.AreEqual("ghi", nt.EmailStalkTemplate);
            Assert.AreEqual("jkl", nt.EmailGreeting);
            Assert.AreEqual("mno", nt.EmailSignature);
            Assert.AreEqual("pqr", nt.EmailStalkReport);
            Assert.AreEqual("stu", nt.EmailStalkReportSubject);
            Assert.AreEqual("vwx", nt.IrcAlertFormat);
            Assert.AreEqual("yza", nt.IrcStalkTagSeparator);
        }

        [Test]
        public void ShouldThrowNullRefOnEmailRcSubject()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates(null, "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailRcTemplate()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", null, "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailStalkTemplate()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", null, "jkl", "mno", "pqr", "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailGreeting()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", null, "mno", "pqr", "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailSignature()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", "jkl", null, "pqr", "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailStalkReport()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", "jkl", "mno", null, "stu", "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefEmailStalkReportSubject()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", "jkl", "mno", "pqr", null, "vwx", "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefIrcAlertFormat()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", "jkl", "mno", "pqr", "stu", null, "yza");
            });
        }

        [Test]
        public void ShouldThrowNullRefIrcStalkTagSeparator()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new NotificationTemplates("abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", null);
            });
        }
    }
}
