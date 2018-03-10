using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class SummaryStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.Expression.Match(rc.EditSummary).Success;
        }

        public new static StalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            var s = new SummaryStalkNode();
            s.SetMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("summary", xmlns);
            e.SetAttribute("value", this.Expression.ToString());
            return e;
        }
        
        public override string ToString()
        {
            return "(summary:\"" + this.Expression + "\")";
        }
        #endregion
    }
}