using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class AndNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return (LeftChildNode.match(rc) && RightChildNode.match(rc));
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("and", xmlns);
            e.AppendChild(LeftChildNode.toXmlFragment(doc, xmlns));
            e.AppendChild(RightChildNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            AndNode s = new AndNode();
            s.LeftChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[0]);
            s.RightChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[1]);
            return s;
        }

        public override string ToString()
        {
            return "(" + LeftChildNode + " && " + RightChildNode + ")";
        }
        #endregion
    }
}
