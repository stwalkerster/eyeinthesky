namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class StalkFactory : StalkConfigFactoryBase, IStalkFactory
    {
        private readonly IIrcClient freenodeClient;
        private readonly IAppConfiguration appConfig;

        public StalkFactory(
            ILogger logger,
            IStalkNodeFactory stalkNodeFactory,
            IIrcClient freenodeClient,
            IAppConfiguration appConfig)
            : base(logger, stalkNodeFactory)
        {
            this.freenodeClient = freenodeClient;
            this.appConfig = appConfig;
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

            // Enabled attribute
            var enabledText = element.GetAttribute("enabled");
            bool enabled;
            if (!bool.TryParse(enabledText, out enabled))
            {
                this.Logger.WarnFormat(
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

            var watchChannel = element.GetAttribute("watchchannel");
            if (string.IsNullOrWhiteSpace(watchChannel))
            {
                watchChannel = this.appConfig.WikimediaChannel;
            }

            var s = new ComplexStalk(
                flag,
                lastUpdateTime,
                lastTriggerTime,
                description,
                expiryTime,
                enabled,
                triggerCount,
                lastMessageId,
                watchChannel);
            
            this.ProcessStalkChildren(element, flag, s);

            return s;
        }

        public XmlElement ToXmlElement(IStalk stalk, XmlDocument doc)
        {
            var e = doc.CreateElement("complexstalk");
            
            e.SetAttribute("flag", stalk.Identifier);
            
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
            
            if (stalk.Description != null)
            {
                e.SetAttribute("description", stalk.Description);
            }
            
            if (stalk.LastMessageId != null)
            {
                e.SetAttribute("lastmessageid", stalk.LastMessageId);
            }

            e.SetAttribute("enabled", XmlConvert.ToString(stalk.IsEnabled));
            
            e.SetAttribute("watchchannel", stalk.WatchChannel);
            
            if (stalk.ExpiryTime != null)
            {
                e.SetAttribute("expiry", XmlConvert.ToString(stalk.ExpiryTime.Value, XmlDateTimeSerializationMode.Utc));
            }

            e.SetAttribute("triggercount", XmlConvert.ToString(stalk.TriggerCount));

            var searchTreeParentElement = doc.CreateElement("searchtree");
            searchTreeParentElement.AppendChild(this.StalkNodeFactory.ToXml(doc, stalk.SearchTree));
            e.AppendChild(searchTreeParentElement);

            var subsElement = doc.CreateElement("subscribers");
            e.AppendChild(subsElement);

            foreach (var user in stalk.Subscribers)
            {
                var u = doc.CreateElement("user");
                u.SetAttribute("mask", user.Mask.ToString());

                if (!user.Subscribed)
                {
                    u.SetAttribute("unsubscribed", XmlConvert.ToString(true));
                }
                
                subsElement.AppendChild(u);
            }
            
            return e;
        }

        protected void ProcessStalkChildren(XmlElement element, string flag, ComplexStalk complexStalk)
        {
            var foundSearchTree = false;

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
                        complexStalk.SetStalkTree(
                            this.StalkNodeFactory.NewFromXmlFragment((XmlElement) xmlElement.FirstChild));
                        foundSearchTree = true;
                        
                        continue;
                    }
                    
                    if (xmlElement.Name == "subscribers")
                    {
                        complexStalk.Subscribers.AddRange(
                            this.PopulateSubscribers(xmlElement.ChildNodes));
                        
                        continue;
                    }

                    this.Logger.DebugFormat("Unrecognised child {0} of stalk {1}", xmlElement.Name, flag);
                }

                if (!foundSearchTree)
                {
                    this.Logger.InfoFormat("Assuming stalk {0} is legacy", flag);
                    complexStalk.SetStalkTree(this.StalkNodeFactory.NewFromXmlFragment((XmlElement) element.FirstChild));
                }
            }
        }

        private IEnumerable<StalkUser> PopulateSubscribers(XmlNodeList xmlElementChildNodes)
        {
            foreach (var elementChildNode in xmlElementChildNodes)
            {
                var e = elementChildNode as XmlElement;
                if (e == null || e.Name != "user")
                {
                    continue;
                }
                
                var mask = e.Attributes["mask"].Value;

                var inverse = false;
                
                if (e.Attributes["unsubscribed"] != null)
                {
                    inverse = XmlConvert.ToBoolean(e.Attributes["unsubscribed"].Value);
                }
                
                // TODO: Hack in a delay for stored $a masks until T1236 is fixed
                if (this.freenodeClient.ExtBanTypes == null)
                {
                    Thread.Sleep(5000);
                }
                
                var ircUserMask = new IrcUserMask(mask, this.freenodeClient);

                yield return new StalkUser(ircUserMask, !inverse);
            }            
        }
    }
}