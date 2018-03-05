using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;

    class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.User).Success;
        }

        public new static StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            UserStalkNode s = new UserStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("user", xmlns);
            e.SetAttribute("value", expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(user:\"" + expression + "\")";
        }

        #endregion
    }
}
