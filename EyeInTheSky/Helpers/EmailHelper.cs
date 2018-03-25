namespace EyeInTheSky.Helpers
{
    using System.Text;
    using Castle.Core.Logging;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

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

        public void SendEmail(string message, string subject)
        {
            if (this.appConfig.EmailConfiguration == null)
            {
                this.logger.Debug("Not sending email; email configuration is disabled");
                return;
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                this.logger.Error("No subject specified in outbound email!");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(message))
            {
                this.logger.Error("No message specified in outbound email!");
                return;
            }

            var builder = new StringBuilder();
            builder.Append(this.templates.EmailGreeting);
            builder.Append(message);
            builder.Append(this.templates.EmailSignature);
            
            this.emailSender.SendEmail(
                this.appConfig.EmailConfiguration.Sender,
                this.appConfig.EmailConfiguration.To,
                subject,
                builder.ToString(),
                this.appConfig.EmailConfiguration.Hostname,
                this.appConfig.EmailConfiguration.Port,
                this.appConfig.EmailConfiguration.Username,
                this.appConfig.EmailConfiguration.Password,
                this.appConfig.EmailConfiguration.Thumbprint);
        }
    }
}