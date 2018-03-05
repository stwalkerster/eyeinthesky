using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace EyeInTheSky
{
    using EyeInTheSky.Model;

    public class StalkList : SortedList<string,ComplexStalk>
    {
        public Stalk search(RecentChange rc)
        {
            foreach (KeyValuePair<string,ComplexStalk> s in this)
            {
                if(s.Value.match(rc))
                {
                    return s.Value;
                }
            }

            return null;
        }

        internal static StalkList fetch(XPathNavigator nav, XmlNamespaceManager nsManager)
        {
            StalkList list = new StalkList();

            XmlDocument xd = new XmlDocument(nav.NameTable);
            xd.LoadXml(nav.OuterXml);
            XmlNode node = xd.ChildNodes[0];
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if(childNode.NodeType != XmlNodeType.Element)
                    continue;

                XmlElement element = (XmlElement) childNode;

                ComplexStalk s = (ComplexStalk)Stalk.newFromXmlElement(element);
                list.Add(s.Flag, s);
            }

            return list;
        }
    }
}
