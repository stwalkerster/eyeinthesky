using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace EyeInTheSky
{
    using System;
    using EyeInTheSky.Model;

    public class StalkList : SortedList<string,ComplexStalk>
    {
        [Obsolete]
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

        public IEnumerable<Stalk> Search(RecentChange rc)
        {
            foreach (KeyValuePair<string,ComplexStalk> s in this)
            {
                if(s.Value.match(rc))
                {
                    yield return s.Value;
                }
            }
        }
        
        internal static StalkList LoadFromXmlFragment(string fragment, XmlNameTable nameTable)
        {
            StalkList list = new StalkList();

            XmlDocument xd = new XmlDocument(nameTable);
            xd.LoadXml(fragment);
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
