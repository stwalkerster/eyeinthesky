using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool match(RecentChange rc)
        {
            return !this.ChildNode.match(rc);
        }

        public static new StalkNode newFromXmlFragment(XmlNode xmlNode)
        {
            NotNode s = new NotNode();
            s.ChildNode = StalkNode.newFromXmlFragment(xmlNode.ChildNodes[0]);
            return s;
        }
        #endregion
    }
}