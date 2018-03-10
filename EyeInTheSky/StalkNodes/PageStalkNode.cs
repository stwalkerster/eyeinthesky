using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class PageStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.Expression.Match(rc.Page).Success;
        }

        public new static StalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            var s = new PageStalkNode();
            s.SetMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }
        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("page", xmlns);
            e.SetAttribute("value", this.Expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(page:\"" + this.Expression + "\")";
        }

        #endregion
    }
}