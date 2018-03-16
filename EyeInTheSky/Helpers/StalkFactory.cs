namespace EyeInTheSky.Helpers
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

        public StalkFactory(ILogger logger)
        {
            this.logger = logger;
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
            }

            // Expiry time
            timeAttribute = element.Attributes["expiry"];
            DateTime? expiryTime = null;
            if (timeAttribute != null)
            {
                expiryTime = this.ParseDate(flag, timeAttribute.Value, "expiry time");
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

            var description = element.GetAttribute("description");

            IStalkNode baseNode = new FalseNode();
            if (element.HasChildNodes)
            {
                baseNode = StalkNode.NewFromXmlFragment(element.FirstChild);
            }

            var s = new ComplexStalk(
                flag,
                lastUpdateTime,
                lastTriggerTime,
                description,
                expiryTime,
                mailEnabled,
                enabled,
                baseNode);

            return s;
        }

        public XmlElement ToXmlElement(IStalk stalk, XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            
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

            e.AppendChild(stalk.SearchTree.ToXmlFragment(doc, xmlns));
            
            return e;
        }
        
        internal DateTime ParseDate(string flagName, string input, string propName)
        {
            DateTime result;

            if (!DateTime.TryParseExact(
                input,
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out result))
            {
                this.logger.WarnFormat("Unknown date format in stalk '{0}' {2}: {1}", flagName, input, propName);

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    var err = string.Format("Failed date parse for stalk '{0}' {2}: {1}", flagName, input, propName);

                    this.logger.Error(err);
                    throw new FormatException(err);
                }
            }

            return result;
        }
    }
}