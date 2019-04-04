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
    using IrcChannel = EyeInTheSky.Model.IrcChannel;

    public class ChannelFactory : ConfigFactoryBase, IChannelFactory
    {
        private readonly IIrcClient freenodeClient;
        private readonly IStalkFactory stalkFactory;

        public ChannelFactory(ILogger logger, IIrcClient freenodeClient, IStalkFactory stalkFactory) : base(logger)
        {
            this.freenodeClient = freenodeClient;
            this.stalkFactory = stalkFactory;
        }

        public IIrcChannel NewFromXmlElement(XmlElement element)
        {
            var name = element.Attributes["name"].Value;

            var guid = Guid.NewGuid();
            var guidAttribute = element.GetAttribute("guid");
            if (!string.IsNullOrWhiteSpace(guidAttribute))
            {
                guid = Guid.Parse(guidAttribute);
            }

            var ircChannel = new IrcChannel(name, guid);

            this.ChildrenFromXml(element, name, ircChannel);

            return ircChannel;
        }

        private void ChildrenFromXml(XmlElement element, string name, IrcChannel ircChannel)
        {
            if (!element.HasChildNodes)
            {
                return;
            }

            var childNodeCollection = element.ChildNodes;

            foreach (XmlNode node in childNodeCollection)
            {
                var xmlElement = node as XmlElement;
                if (xmlElement == null)
                {
                    continue;
                }

                if (xmlElement.Name == "users")
                {
                    ircChannel.Users.Clear();

                    foreach (var childNode in xmlElement.ChildNodes)
                    {
                        var childElement = childNode as XmlElement;
                        if (childElement == null)
                        {
                            continue;
                        }

                        ircChannel.Users.Add(this.NewUserFromXmlElement(childElement));
                    }

                    continue;
                }

                if (xmlElement.Name == "stalks")
                {
                    ircChannel.Stalks.Clear();

                    foreach (var childNode in xmlElement.ChildNodes)
                    {
                        var childElement = childNode as XmlElement;
                        if (childElement == null)
                        {
                            continue;
                        }

                        var stalk = this.stalkFactory.NewFromXmlElement(childElement);
                        stalk.Channel = ircChannel.Identifier;
                        ircChannel.Stalks.Add(stalk.Identifier, stalk);
                    }

                    continue;
                }

                this.Logger.DebugFormat("Unrecognised child {0} of channel {1}", xmlElement.Name, name);
            }
        }

        private ChannelUser NewUserFromXmlElement(XmlElement element)
        {
            var mask = element.Attributes["mask"].Value;

            // Flags
            var localFlags = element.GetAttribute("localflags");
            if (string.IsNullOrWhiteSpace(localFlags))
            {
                localFlags = null;
            }

            var subscribedText = element.GetAttribute("subscribed");
            bool subscribed;
            if (!bool.TryParse(subscribedText, out subscribed))
            {
                subscribed = false;
            }

            // TODO: Hack in a delay for stored $a masks until T1236 is fixed
            if (this.freenodeClient.ExtBanTypes == null)
            {
                Thread.Sleep(5000);
            }

            var userMask = new IrcUserMask(mask, this.freenodeClient);

            return new ChannelUser(userMask)
            {
                LocalFlags = localFlags,
                Subscribed = subscribed
            };
        }

        public XmlElement ToXmlElement(IIrcChannel item, XmlDocument doc)
        {
            var e = doc.CreateElement("channel");
            e.SetAttribute("name", item.Identifier);
            e.SetAttribute("guid", item.Guid.ToString());

            var usersElement = doc.CreateElement("users");
            e.AppendChild(usersElement);
            var stalksElement = doc.CreateElement("stalks");
            e.AppendChild(stalksElement);

            foreach (var channelUser in item.Users)
            {
                var u = doc.CreateElement("user");
                u.SetAttribute("mask", channelUser.Mask.ToString());
                bool saveable = false;

                if (!string.IsNullOrEmpty(channelUser.LocalFlags))
                {
                    u.SetAttribute("localflags", channelUser.LocalFlags);
                    saveable = true;
                }

                if (channelUser.Subscribed)
                {
                    u.SetAttribute("subscribed", XmlConvert.ToString(channelUser.Subscribed));
                    saveable = true;
                }

                if (saveable)
                {
                    usersElement.AppendChild(u);
                }
            }

            foreach (var stalk in item.Stalks)
            {
                var xmlElement = this.stalkFactory.ToXmlElement(stalk.Value, doc);
                stalksElement.AppendChild(xmlElement);
            }

            return e;
        }
    }
}