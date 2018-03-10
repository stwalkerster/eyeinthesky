namespace EyeInTheSky.Model
{
    using System.Collections.Generic;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;

    public class StalkList : SortedList<string, IStalk>
    {
        public IEnumerable<IStalk> Search(IRecentChange rc)
        {
            foreach (var s in this)
            {
                if(s.Value.Match(rc))
                {
                    yield return s.Value;
                }
            }
        }
        
        internal static StalkList LoadFromXmlFragment(string fragment, XmlNameTable nameTable)
        {
            var list = new StalkList();

            var xd = new XmlDocument(nameTable);
            xd.LoadXml(fragment);
            var node = xd.ChildNodes[0];
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                var element = (XmlElement) childNode;

                var stalk = (ComplexStalk) ComplexStalk.NewFromXmlElement(element);
                list.Add(stalk.Flag, stalk);
            }

            return list;
        }
    }
}
