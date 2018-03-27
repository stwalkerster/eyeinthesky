namespace EyeInTheSky.Services.Interfaces
{
    public interface IEmailHelper
    {
        string SendEmail(string message, string subject, string inReplyTo);
    }
}