using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return !this.ChildNode.Match(rc);
        }
        
        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("not", xmlns);
            e.AppendChild(this.ChildNode.ToXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "(!" + this.ChildNode +  ")";
        }
        #endregion
    }
}