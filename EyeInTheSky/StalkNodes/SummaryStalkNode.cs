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

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            SummaryStalkNode s = new SummaryStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        #endregion
    }
}