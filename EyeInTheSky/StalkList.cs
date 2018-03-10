using System.Collections.Generic;
using System.Xml;

namespace EyeInTheSky
{
    using EyeInTheSky.Model.Interfaces;

    public class StalkList : SortedList<string, ComplexStalk>
    {
        public IEnumerable<IStalk> Search(IRecentChange rc)
        {
            foreach (KeyValuePair<string, ComplexStalk> s in this)
            {
                if(s.Value.Match(rc))
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

                ComplexStalk s = (ComplexStalk)ComplexStalk.NewFromXmlElement(element);
                list.Add(s.Flag, s);
            }

            return list;
        }
    }
}
