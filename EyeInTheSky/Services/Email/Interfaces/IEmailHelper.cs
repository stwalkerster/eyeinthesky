namespace EyeInTheSky.Services.Email.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public interface IEmailHelper
    {
        string SendEmail(
            string message,
            string subject,
            string inReplyTo,
            IBotUser recipient,
            Dictionary<string, string> extraHeaders);
    }
}