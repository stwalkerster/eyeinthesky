namespace EyeInTheSky.Helpers.Interfaces
{
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
            string inReplyTo);
    }
}