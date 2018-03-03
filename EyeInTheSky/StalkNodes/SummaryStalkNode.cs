using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class SummaryStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.EditSummary).Success;
        }

        public new static StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            SummaryStalkNode s = new SummaryStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("summary", xmlns);
            e.SetAttribute("value", expression.ToString());
            return e;
        }
        public override string ToString()
        {
            return "(summary:\" " + expression + "\")";
        }
        #endregion
    }
}