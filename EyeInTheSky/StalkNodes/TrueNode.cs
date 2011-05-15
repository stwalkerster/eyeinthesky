﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class TrueNode : LogicalNode
    {
        public override bool match(RecentChange rc)
        {
            return true;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("true");
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            return new TrueNode();
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}