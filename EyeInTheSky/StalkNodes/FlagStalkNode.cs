﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class FlagStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return expression.Match(rc.EditFlags).Success;
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            FlagStalkNode s = new FlagStalkNode();
            s.setMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("flag", xmlns);
            e.SetAttribute("value", expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(flag:\" " + expression + "\")";
        }
        #endregion
    }
}