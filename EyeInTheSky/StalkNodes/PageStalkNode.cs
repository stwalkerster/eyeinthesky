using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class PageStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.Page).Success;
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            PageStalkNode s = new PageStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        #endregion
    }
}