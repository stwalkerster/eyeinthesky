﻿namespace EyeInTheSky.Helpers
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;

    public class StalkFactory
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
            var lastUpdateTimeText = timeAttribute == null
                ? DateTime.Now.ToString(CultureInfo.InvariantCulture)
                : timeAttribute.Value;
            DateTime lastUpdateTime;
            this.ParseDate(flag, lastUpdateTimeText, out lastUpdateTime, "last update time");

            // Last trigger time
            timeAttribute = element.Attributes["lasttrigger"];
            var lastTriggerTimeText = timeAttribute == null
                ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture)
                : timeAttribute.Value;
            DateTime lastTriggerTime;
            this.ParseDate(flag, lastTriggerTimeText, out lastTriggerTime, "last trigger time");

            // Expiry time
            timeAttribute = element.Attributes["expiry"];
            var expiryTimeText = timeAttribute == null
                ? DateTime.MaxValue.ToString(CultureInfo.InvariantCulture)
                : timeAttribute.Value;
            DateTime expiryTime;
            this.ParseDate(flag, expiryTimeText, out expiryTime, "expiry time");

            // Email attribute
            var immediateMailText = element.GetAttribute("immediatemail");
            bool mailEnabled;
            if (!bool.TryParse(immediateMailText, out mailEnabled))
            {
                this.logger.ErrorFormat(
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

            StalkNode baseNode = new FalseNode();
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

        private void ParseDate(string flagName, string input, out DateTime result, string propName)
        {
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
        }
    }
}