﻿namespace EyeInTheSky.Helpers
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;

    public class StalkFactory : IStalkFactory
    {
        private readonly ILogger logger;
        private readonly IStalkNodeFactory stalkNodeFactory;

        public StalkFactory(ILogger logger, IStalkNodeFactory stalkNodeFactory)
        {
            this.logger = logger;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        public IStalk NewFromXmlElement(XmlElement element)
        {
            var flag = element.Attributes["flag"].Value;

            // Last update time
            var timeAttribute = element.Attributes["lastupdate"];
            DateTime? lastUpdateTime = null;
            if (timeAttribute != null)
            {
                lastUpdateTime = this.ParseDate(flag, timeAttribute.Value, "last update time");
            }
            
            // Last trigger time
            timeAttribute = element.Attributes["lasttrigger"];
            DateTime? lastTriggerTime = null;
            if (timeAttribute != null)
            {
                lastTriggerTime = this.ParseDate(flag, timeAttribute.Value, "last trigger time");
                
                // backwards compat
                if (lastTriggerTime == new DateTime(1970, 1, 1, 0, 0, 0))
                {
                    lastTriggerTime = null;
                }
            }

            // Expiry time
            timeAttribute = element.Attributes["expiry"];
            DateTime? expiryTime = null;
            if (timeAttribute != null)
            {
                expiryTime = this.ParseDate(flag, timeAttribute.Value, "expiry time");
                
                // backwards compat
                if (expiryTime == new DateTime(9999, 12, 31, 23, 59, 59))
                {
                    expiryTime = null;
                }
            }

            // Email attribute
            var immediateMailText = element.GetAttribute("immediatemail");
            bool mailEnabled;
            if (!bool.TryParse(immediateMailText, out mailEnabled))
            {
                this.logger.WarnFormat(
                    "Unable to parse immediatemail attribute value '{1}' for stalk {0}. Defaulting to enabled.",
                    flag,
                    immediateMailText);
                mailEnabled = true;
            }

            // Enabled attribute
            var enabledText = element.GetAttribute("enabled");
            bool enabled;
            if (!bool.TryParse(enabledText, out enabled))
            {
                this.logger.WarnFormat(
                    "Unable to parse enabled attribute value '{1}' for stalk {0}. Defaulting to enabled.",
                    flag,
                    enabledText);
                enabled = true;
            }

            var triggercountText = element.GetAttribute("triggercount");
            int triggerCount;
            int.TryParse(triggercountText, out triggerCount);

            var description = element.GetAttribute("description");
            if (string.IsNullOrWhiteSpace(description))
            {
                description = null;
            }

            IStalkNode baseNode = new FalseNode();
            if (element.HasChildNodes)
            {
                baseNode = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) element.FirstChild);
            }

            var s = new ComplexStalk(
                flag,
                lastUpdateTime,
                lastTriggerTime,
                description,
                expiryTime,
                mailEnabled,
                enabled,
                triggerCount,
                baseNode);

            return s;
        }

        public XmlElement ToXmlElement(IStalk stalk, XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("complexstalk", xmlns);
            
            e.SetAttribute("flag", stalk.Flag);
            
            if (stalk.LastUpdateTime != null)
            {
                e.SetAttribute(
                    "lastupdate",
                    XmlConvert.ToString(stalk.LastUpdateTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            if (stalk.LastTriggerTime != null)
            {
                e.SetAttribute(
                    "lasttrigger",
                    XmlConvert.ToString(stalk.LastTriggerTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            e.SetAttribute("immediatemail", XmlConvert.ToString(stalk.MailEnabled));
            
            if (stalk.Description != null)
            {
                e.SetAttribute("description", stalk.Description);
            }

            e.SetAttribute("enabled", XmlConvert.ToString(stalk.IsEnabled));
            
            if (stalk.ExpiryTime != null)
            {
                e.SetAttribute("expiry", XmlConvert.ToString(stalk.ExpiryTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            e.SetAttribute("triggercount", XmlConvert.ToString(stalk.TriggerCount));

            e.AppendChild(this.stalkNodeFactory.ToXml(doc, xmlns, stalk.SearchTree));
            
            return e;
        }
        
        internal DateTime ParseDate(string flagName, string input, string propName)
        {
            DateTime result;

            try
            {
                result = XmlConvert.ToDateTime(input, XmlDateTimeSerializationMode.Utc);
            }
            catch (FormatException ex)
            {
                this.logger.WarnFormat("Unknown date format in stalk '{0}' {2}: {1}", flagName, input, propName);

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    var err = string.Format("Failed date parse for stalk '{0}' {2}: {1}", flagName, input, propName);

                    this.logger.Error(err);
                    throw new FormatException(err, ex);
                }
            }

            return result;
        }
    }
}