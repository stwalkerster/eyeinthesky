namespace EyeInTheSky.Services.Email
{
    using System.Collections.Generic;
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Email.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;

    public class EmailHelper : IEmailHelper
    {
        private readonly ILogger logger;
        private readonly INotificationTemplates templates;
        private readonly IIrcClient freenodeClient;
        private readonly IAppConfiguration appConfig;
        private readonly IEmailSender emailSender;

        public EmailHelper(IAppConfiguration appConfig, IEmailSender emailSender, ILogger logger, INotificationTemplates templates, IIrcClient freenodeClient)
        {
            this.logger = logger;
            this.templates = templates;
            this.freenodeClient = freenodeClient;
            this.appConfig = appConfig;
            this.emailSender = emailSender;
        }

        public string SendEmail(
            string message,
            string subject,
            string inReplyTo,
            IBotUser recipient,
            Dictionary<string, string> extraHeaders)
        {
            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Debug("Not sending email; email configuration is disabled");
                return null;
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                this.logger.Error("No subject specified in outbound email!");
                return null;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                this.logger.Error("No message specified in outbound email!");
                return null;
            }

            if (string.IsNullOrWhiteSpace(recipient.EmailAddress))
            {
                this.logger.Info("No recipient address provided!");
                return null;
            }

            var signaturePattern = string.Format(
                this.templates.EmailSignature,
                recipient.Identifier.Substring(3),
                this.freenodeClient.Nickname,
                this.appConfig.CommandPrefix,
                this.appConfig.PrivacyPolicy);

            var builder = new StringBuilder();
            builder.Append(this.templates.EmailGreeting);
            builder.Append(message);
            builder.Append(signaturePattern);

            var subjectPrefix = this.appConfig.EmailConfiguration.SubjectPrefix;

            subjectPrefix = subjectPrefix == null ? string.Empty : subjectPrefix.TrimEnd() + " ";

            return this.emailSender.SendEmail(
                this.appConfig.EmailConfiguration.Sender,
                recipient.EmailAddress,
                string.Format("{0}{1}", subjectPrefix, subject),
                builder.ToString(),
                this.appConfig.EmailConfiguration.Hostname,
                this.appConfig.EmailConfiguration.Port,
                this.appConfig.EmailConfiguration.Username,
                this.appConfig.EmailConfiguration.Password,
                this.appConfig.EmailConfiguration.Thumbprint,
                inReplyTo,
                extraHeaders
            );
        }
    }
}