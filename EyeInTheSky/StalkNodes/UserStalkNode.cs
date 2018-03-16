using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.Expression.Match(rc.User).Success;
        }

        public new static IStalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            var s = new UserStalkNode();
            s.SetMatchExpression(xmlNode.Attributes["value"].Value);
            return s;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("user", xmlns);
            e.SetAttribute("value", this.Expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(user:\"" + this.Expression + "\")";
        }

        #endregion
    }
}
