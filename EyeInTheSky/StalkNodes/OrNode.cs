using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class OrNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return (LeftChildNode.match(rc) || RightChildNode.match(rc));
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            OrNode s = new OrNode();
            s.LeftChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[0]);
            s.RightChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[1]);
            return s;
        }
        #endregion
    }
}