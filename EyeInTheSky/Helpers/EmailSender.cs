namespace EyeInTheSky.Helpers
{
    using System.Security.Cryptography.X509Certificates;
    using EyeInTheSky.Helpers.Interfaces;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    public class EmailSender : IEmailSender
    {
        public void SendEmail(string sender,
            string to,
            string subject,
            string body,
            string hostname,
            int port,
            string username,
            string password,
            string thumbprint)
        {
            var mailMessage = new MimeMessage();

            mailMessage.From.Add(MailboxAddress.Parse(sender));
            mailMessage.To.Add(MailboxAddress.Parse(to));
            mailMessage.Subject = subject;

            mailMessage.Body = new TextPart("plain")
            {
                Text = body
            };

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
        }
    }
}