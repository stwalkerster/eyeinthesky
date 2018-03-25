namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    
    public class NotificationTemplates : INotificationTemplates
    {
        public NotificationTemplates(string emailRcSubject,
            string emailRcTemplate,
            string emailStalkTemplate,
            string emailGreeting,
            string emailSignature,
            string emailStalkReport,
            string emailStalkReportSubject,
            string ircAlertFormat,
            string ircStalkTagSeparator)
        {
            if (emailRcSubject == null)
            {
                throw new ArgumentNullException("emailRcSubject");
            }

            if (emailRcTemplate == null)
            {
                throw new ArgumentNullException("emailRcTemplate");
            }

            if (emailStalkTemplate == null)
            {
                throw new ArgumentNullException("emailStalkTemplate");
            }

            if (emailGreeting == null)
            {
                throw new ArgumentNullException("emailGreeting");
            }

            if (emailSignature == null)
            {
                throw new ArgumentNullException("emailSignature");
            }

            if (emailStalkReport == null)
            {
                throw new ArgumentNullException("emailStalkReport");
            }

            if (emailStalkReportSubject == null)
            {
                throw new ArgumentNullException("emailStalkReportSubject");
            }

            if (ircAlertFormat == null)
            {
                throw new ArgumentNullException("ircAlertFormat");
            }

            if (ircStalkTagSeparator == null)
            {
                throw new ArgumentNullException("ircStalkTagSeparator");
            }

            this.EmailRcSubject = emailRcSubject;
            this.EmailRcTemplate = emailRcTemplate;
            this.EmailStalkTemplate = emailStalkTemplate;
            this.EmailGreeting = emailGreeting;
            this.EmailSignature = emailSignature;
            this.EmailStalkReport = emailStalkReport;
            this.EmailStalkReportSubject = emailStalkReportSubject;
            this.IrcAlertFormat = ircAlertFormat;
            this.IrcStalkTagSeparator = ircStalkTagSeparator;
        }

        public string EmailRcSubject { get; private set; }
        public string EmailRcTemplate { get; private set; }
        public string EmailStalkTemplate { get; private set; }
        public string EmailGreeting { get; private set; }
        public string EmailSignature { get; private set; }
        public string EmailStalkReport { get; private set; }
        public string EmailStalkReportSubject { get; private set; }
        
        public string IrcAlertFormat { get; private set; }
        public string IrcStalkTagSeparator { get; private set; }
    }
}