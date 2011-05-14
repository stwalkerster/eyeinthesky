using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace EyeInTheSky
{
    public class StalkList : SortedList<string,Stalk>
    {
        public Stalk search(RecentChange rc)
        {
            foreach (KeyValuePair<string,Stalk> s in this)
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

            XmlParserContext xpc = new XmlParserContext(nav.NameTable, nsManager, "en_GB", XmlSpace.Default);
            XmlTextReader xr = new XmlTextReader(nav.OuterXml, XmlNodeType.Element, xpc);
            if (!xr.Read())
                throw new XmlException();

            if (xr.Name != "stalks")
                throw new XmlException();

            while (xr.Read() && xr.Name != "stalks")
            {
                if (xr.NodeType == XmlNodeType.Whitespace)
                    continue;
                if (xr.Name == "stalk")
                { // simple stalk

                }

            }

            return list;
        }
    }
}
