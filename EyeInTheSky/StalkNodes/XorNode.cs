using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;

    class XorNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return (LeftChildNode.match(rc) ^ RightChildNode.match(rc));
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            XorNode s = new XorNode();
            s.LeftChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[0]);
            s.RightChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[1]);
            return s;
        }
        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("xor", xmlns);
            e.AppendChild(LeftChildNode.toXmlFragment(doc, xmlns));
            e.AppendChild(RightChildNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "(^:" + LeftChildNode + RightChildNode + ")";
        }
        #endregion
    }
}