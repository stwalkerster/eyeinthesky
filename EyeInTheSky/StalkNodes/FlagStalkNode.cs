using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class FlagStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.Expression.Match(rc.EditFlags).Success;
        }
        
        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("flag", xmlns);
            e.SetAttribute("value", this.Expression.ToString());
            return e;
        }

        public override string ToString()
        {
            return "(flag:\"" + this.Expression + "\")";
        }
        #endregion
    }
}
