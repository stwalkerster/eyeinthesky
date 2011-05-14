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




            return list;
        }
    }
}
