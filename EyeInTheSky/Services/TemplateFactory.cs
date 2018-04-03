namespace EyeInTheSky.Services
{
    using System;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    
    public class TemplateFactory : ConfigFactoryBase, ITemplateFactory
    {
        public TemplateFactory(ILogger logger, IStalkNodeFactory stalkNodeFactory) : base(logger, stalkNodeFactory)
        {
        }
        
        public ITemplate NewFromXmlElement(XmlElement element)
        {
            var flag = element.Attributes["flag"].Value;

            // Last update time
            var timeAttribute = element.Attributes["lastupdate"];
            DateTime? lastUpdateTime = null;
            if (timeAttribute != null)
            {
                lastUpdateTime = this.ParseDate(flag, timeAttribute.Value, "last update time");
            }

            // Email attribute
            var immediateMailText = element.GetAttribute("immediatemail");
            bool mailEnabled;
            if (!bool.TryParse(immediateMailText, out mailEnabled))
            {
                this.Logger.WarnFormat(
                    "Unable to parse immediatemail attribute value '{1}' for stalk {0}. Defaulting to enabled.",
                    flag,
                    immediateMailText);
                mailEnabled = true;
            }

            // Enabled attribute
            var stalkEnabledText = element.GetAttribute("stalkenabled");
            bool stalkEnabled;
            if (!bool.TryParse(stalkEnabledText, out stalkEnabled))
            {
                this.Logger.WarnFormat(
                    "Unable to parse stalkenabled attribute value '{1}' for stalk {0}. Defaulting to enabled.",
                    flag,
                    stalkEnabledText);
                stalkEnabled = true;
            }

            // Enabled attribute
            var templateEnabledText = element.GetAttribute("templateenabled");
            bool templateEnabled;
            if (!bool.TryParse(templateEnabledText, out templateEnabled))
            {
                this.Logger.WarnFormat(
                    "Unable to parse templateenabled attribute value '{1}' for stalk {0}. Defaulting to enabled.",
                    flag,
                    stalkEnabledText);
                templateEnabled = true;
            }
            
            var description = element.GetAttribute("description");
            if (string.IsNullOrWhiteSpace(description))
            {
                description = null;
            }

            var expiryDurationAttribute = element.GetAttribute("expiryduration");
            TimeSpan? expiryDuration = null;
            if (!string.IsNullOrWhiteSpace(expiryDurationAttribute))
            {
                expiryDuration = XmlConvert.ToTimeSpan(expiryDurationAttribute);
            }
            
            var baseNode = this.GetStalkTreeFromXml(element, flag);

            var t = new Template(
                flag,
                templateEnabled,
                stalkEnabled,
                mailEnabled,
                description,
                lastUpdateTime,
                expiryDuration,
                baseNode);

            return t;
        }

        public XmlElement ToXmlElement(ITemplate stalk, XmlDocument doc)
        {
            var e = doc.CreateElement("template");
            
            e.SetAttribute("flag", stalk.Flag);
            
            if (stalk.LastUpdateTime != null)
            {
                e.SetAttribute(
                    "lastupdate",
                    XmlConvert.ToString(stalk.LastUpdateTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            if (stalk.Description != null)
            {
                e.SetAttribute("description", stalk.Description);
            }
            
            e.SetAttribute("immediatemail", XmlConvert.ToString(stalk.MailEnabled));
            e.SetAttribute("stalkenabled", XmlConvert.ToString(stalk.StalkIsEnabled));
            e.SetAttribute("templateenabled", XmlConvert.ToString(stalk.TemplateIsEnabled));

            if (stalk.ExpiryDuration.HasValue)
            {
                e.SetAttribute("expiryduration", XmlConvert.ToString(stalk.ExpiryDuration.Value));
            }
            
            var searchTreeParentElement = doc.CreateElement("searchtree");
            searchTreeParentElement.AppendChild(this.StalkNodeFactory.ToXml(doc, stalk.SearchTree));
            
            e.AppendChild(searchTreeParentElement);

            return e;
        }
    }
}