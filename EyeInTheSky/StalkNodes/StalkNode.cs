using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    public abstract class StalkNode
    {
        abstract public bool match(RecentChange rc);

        public static StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "and":
                    return AndNode.newFromXmlFragment(xmlNode);
                case "or":
                    return OrNode.newFromXmlFragment(xmlNode);
                case "not":
                    return NotNode.newFromXmlFragment(xmlNode);
                case "true":
                    return TrueNode.newFromXmlFragment(xmlNode);
                case "false":
                    return FalseNode.newFromXmlFragment(xmlNode);
                case "user":
                    return UserStalkNode.newFromXmlFragment(xmlNode);
                case "page":
                    return PageStalkNode.newFromXmlFragment(xmlNode);
                case "summary":
                    return SummaryStalkNode.newFromXmlFragment(xmlNode);
                default:
                    throw new XmlException();
            }
        }

        public abstract XmlElement toXmlFragment(XmlDocument doc, string xmlns);
    }
}
