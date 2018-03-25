namespace EyeInTheSky.Helpers.Interfaces
{
    public interface IEmailHelper
    {
        void SendEmail(string message, string subject);
    }
}