namespace EyeInTheSky.Services
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
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

            var lastMessageId = element.GetAttribute("lastmessageid");
            if (string.IsNullOrWhiteSpace(lastMessageId))
            {
                lastMessageId = null;
            }

            IStalkNode baseNode = null;
            
            if (element.HasChildNodes)
            {
                var childNodeCollection = element.ChildNodes;

                foreach (XmlNode node in childNodeCollection)
                {
                    var xmlElement = node as XmlElement;
                    if (xmlElement == null)
                    {
                        continue;
                    }

                    if (xmlElement.Name == "searchtree")
                    {
                        baseNode = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) xmlElement.FirstChild);
                        continue;
                    }
                    
                    this.logger.DebugFormat("Unrecognised child {0} of stalk {1}", xmlElement.Name, flag);
                }

                if (baseNode == null)
                {
                    this.logger.InfoFormat("Assuming stalk {0} is legacy", flag);
                    baseNode = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) element.FirstChild);
                }
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
                lastMessageId,
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
            
            if (stalk.LastMessageId != null)
            {
                e.SetAttribute("lastmessageid", stalk.LastMessageId);
            }

            e.SetAttribute("enabled", XmlConvert.ToString(stalk.IsEnabled));
            
            if (stalk.ExpiryTime != null)
            {
                e.SetAttribute("expiry", XmlConvert.ToString(stalk.ExpiryTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            e.SetAttribute("triggercount", XmlConvert.ToString(stalk.TriggerCount));

            var searchTreeParentElement = doc.CreateElement("searchtree", xmlns);
            searchTreeParentElement.AppendChild(this.stalkNodeFactory.ToXml(doc, xmlns, stalk.SearchTree));
            
            e.AppendChild(searchTreeParentElement);
            
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