namespace EyeInTheSky.Services
{
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class EmailHelper : IEmailHelper
    {
        private readonly ILogger logger;
        private readonly INotificationTemplates templates;
        private readonly IAppConfiguration appConfig;
        private readonly IEmailSender emailSender;

        public EmailHelper(IAppConfiguration appConfig, IEmailSender emailSender, ILogger logger, INotificationTemplates templates)
        {
            this.logger = logger;
            this.templates = templates;
            this.appConfig = appConfig;
            this.emailSender = emailSender;
        }

        public string SendEmail(string message, string subject, string inReplyTo)
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

            var builder = new StringBuilder();
            builder.Append(this.templates.EmailGreeting);
            builder.Append(message);
            builder.Append(this.templates.EmailSignature);
            
            return this.emailSender.SendEmail(
                this.appConfig.EmailConfiguration.Sender,
                this.appConfig.EmailConfiguration.To,
                subject,
                builder.ToString(),
                this.appConfig.EmailConfiguration.Hostname,
                this.appConfig.EmailConfiguration.Port,
                this.appConfig.EmailConfiguration.Username,
                this.appConfig.EmailConfiguration.Password,
                this.appConfig.EmailConfiguration.Thumbprint,
                inReplyTo);
        }
    }
}