namespace EyeInTheSky.Services.Email.Interfaces
{
    using System.Collections.Generic;

    public interface IEmailSender
    {
        string SendEmail(string sender,
            string to,
            string subject,
            string body,
            string hostname,
            int port,
            string username,
            string password,
            string thumbprint,
            string inReplyTo,
            Dictionary<string, string> extraHeaders);
    }
}