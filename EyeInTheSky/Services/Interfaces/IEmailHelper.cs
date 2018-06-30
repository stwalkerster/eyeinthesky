namespace EyeInTheSky.Services.Interfaces
{
    using EyeInTheSky.Model.Interfaces;

    public interface IEmailHelper
    {
        string SendEmail(string message, string subject, string inReplyTo, IBotUser recipient);
    }
}