using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;

    class PageStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.Page).Success;
        }

        public new static StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            PageStalkNode s = new PageStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("page", xmlns);
            e.SetAttribute("value", expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(page:\"" + expression + "\")";
        }

        #endregion
    }
}