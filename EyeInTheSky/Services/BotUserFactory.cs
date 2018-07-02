namespace EyeInTheSky.Services
{
    using System;
    using System.Threading;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class BotUserFactory : ConfigFactoryBase, IBotUserFactory
    {
        private readonly IIrcClient freenodeClient;

        public BotUserFactory(ILogger logger, IIrcClient freenodeClient) : base(logger)
        {
            this.freenodeClient = freenodeClient;
        }

        public IBotUser NewFromXmlElement(XmlElement element)
        {
            var mask = element.Attributes["mask"].Value;

            // Email attribute
            var email = element.GetAttribute("mail");
            if (string.IsNullOrWhiteSpace(email))
            {
                email = null;
            }

            // Flags
            var globalflags = element.GetAttribute("globalflags");
            if (string.IsNullOrWhiteSpace(globalflags))
            {
                globalflags = null;
            }

            // Email confirmation hash
            var confirmationHash = element.GetAttribute("mailconfirmhash");
            if (string.IsNullOrWhiteSpace(confirmationHash))
            {
                confirmationHash = null;
            }

            // Deletion confirmation hash
            var deleteConfirmationHash = element.GetAttribute("deleteconfirmhash");
            if (string.IsNullOrWhiteSpace(deleteConfirmationHash))
            {
                deleteConfirmationHash = null;
            }

            // Mail confirmed
            var mailConfirmedText = element.GetAttribute("mailconfirmed");
            bool mailConfirmed;
            if (!bool.TryParse(mailConfirmedText, out mailConfirmed))
            {
                mailConfirmed = false;
            }

            var timeAttribute = element.Attributes["deletetimestamp"];
            DateTime? deleteTimestamp = null;
            if (timeAttribute != null)
            {
                deleteTimestamp = this.ParseDate(mask, timeAttribute.Value, "delete confirmation timestamp");
            }

            timeAttribute = element.Attributes["emailtimestamp"];
            DateTime? emailTimestamp = null;
            if (timeAttribute != null)
            {
                emailTimestamp = this.ParseDate(mask, timeAttribute.Value, "email confirmation timestamp");
            }

            // TODO: Hack in a delay for stored $a masks until T1236 is fixed
            if (this.freenodeClient.ExtBanTypes == null || this.freenodeClient.ExtBanDelimiter == null)
            {
                Thread.Sleep(5000);
            }
            
            var userMask = new IrcUserMask(mask, this.freenodeClient);
            
            var u = new BotUser(
                userMask,
                globalflags,
                email,
                confirmationHash,
                deleteConfirmationHash,
                mailConfirmed,
                emailTimestamp,
                deleteTimestamp);

            return u;
        }

        public XmlElement ToXmlElement(IBotUser item, XmlDocument doc)
        {
            var e = doc.CreateElement("user");

            e.SetAttribute("mask", item.Identifier);

            if (item.GlobalFlags != null)
            {
                e.SetAttribute("globalflags", item.GlobalFlags);
            }

            if (item.EmailAddress != null)
            {
                e.SetAttribute("mail", item.EmailAddress);
                e.SetAttribute("mailconfirmed", XmlConvert.ToString(item.EmailAddressConfirmed));
            }

            if (item.EmailConfirmationToken != null)
            {
                e.SetAttribute("mailconfirmhash", item.EmailConfirmationToken);
            }

            if (item.DeletionConfirmationToken != null)
            {
                e.SetAttribute("deleteconfirmhash", item.DeletionConfirmationToken);
            }

            if (item.DeletionConfirmationTimestamp != null)
            {
                e.SetAttribute(
                    "deletetimestamp",
                    XmlConvert.ToString(item.DeletionConfirmationTimestamp.Value, XmlDateTimeSerializationMode.Utc));
            }

            if (item.EmailConfirmationTimestamp != null)
            {
                e.SetAttribute(
                    "emailtimestamp",
                    XmlConvert.ToString(item.EmailConfirmationTimestamp.Value, XmlDateTimeSerializationMode.Utc));
            }

            return e;
        }
    }
}