using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.User).Success;
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            UserStalkNode s = new UserStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        #endregion
    }
}
