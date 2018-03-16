using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class XorNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.LeftChildNode.Match(rc) ^ this.RightChildNode.Match(rc);
        }
        
        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("xor", xmlns);
            e.AppendChild(this.LeftChildNode.ToXmlFragment(doc, xmlns));
            e.AppendChild(this.RightChildNode.ToXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "(^:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}