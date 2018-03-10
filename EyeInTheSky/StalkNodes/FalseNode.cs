using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

    class FalseNode : LogicalNode
    {
        public override bool match(IRecentChange rc)
        {
            return false;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("false",xmlns);
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            return new FalseNode();
        }

        public override string ToString()
        {
            return "(false)";
        }
    }
}
