using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;

    class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return !this.ChildNode.match(rc);
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            NotNode s = new NotNode();
            s.ChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[0]);
            return s;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("not", xmlns);
            e.AppendChild(ChildNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "(!" + ChildNode +  ")";
        }
        #endregion
    }
}