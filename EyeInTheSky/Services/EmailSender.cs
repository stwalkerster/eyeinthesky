namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using EyeInTheSky.Services.Interfaces;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using MimeKit.Utils;

    public class EmailSender : IEmailSender
    {
        public string SendEmail(string sender,
            string to,
            string subject,
            string body,
            string hostname,
            int port,
            string username,
            string password,
            string thumbprint,
            string inReplyTo,
            Dictionary<string, string> extraHeaders)
        {
            var mailMessage = new MimeMessage();

            mailMessage.From.Add(MailboxAddress.Parse(sender));
            mailMessage.To.Add(MailboxAddress.Parse(to));
            mailMessage.Subject = subject;

            mailMessage.Body = new TextPart("plain")
            {
                Text = body
            };

            mailMessage.MessageId = MimeUtils.GenerateMessageId("eyeinthesky.im");

            if (!string.IsNullOrWhiteSpace(inReplyTo))
            {
                mailMessage.InReplyTo = inReplyTo;
            }

            if (extraHeaders != null && extraHeaders.Any())
            {
                foreach (var kvp in extraHeaders)
                {
                    mailMessage.Headers.Add("X-EITS-" + kvp.Key, kvp.Value);
                }
            }

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback =
                    (o, cert, chain, errors) =>
                    {
                        if (thumbprint == "any")
                        {
                            return true;
                        }

                        return ((X509Certificate2) cert).Thumbprint == thumbprint;
                    };

                client.Connect(hostname, port, SecureSocketOptions.StartTls);

                if (username != null && password != null)
                {
                    client.Authenticate(username, password);
                }

                client.Send(mailMessage);
                client.Disconnect(true);
            }

            return mailMessage.MessageId;
        }
    }
}