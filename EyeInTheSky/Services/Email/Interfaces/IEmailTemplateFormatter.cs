namespace EyeInTheSky.Services.Email.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public interface IEmailTemplateFormatter
    {
        string FormatStalkListForEmail(IEnumerable<IStalk> stalks, IBotUser botUser);

        string FormatRecentChangeStalksForEmail(IEnumerable<IStalk> stalks, IRecentChange rc, IBotUser botUser);

    }
}