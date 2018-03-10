using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;

    class TrueNode : LogicalNode
    {
        public override bool match(IRecentChange rc)
        {
            return true;
        }

        public override XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("true", xmlns);
        }

        public new static StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            return new TrueNode();
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}
