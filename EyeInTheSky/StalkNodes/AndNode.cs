using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class AndNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return (this.LeftChildNode.Match(rc) && this.RightChildNode.Match(rc));
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("and", xmlns);
            e.AppendChild(this.LeftChildNode.ToXmlFragment(doc, xmlns));
            e.AppendChild(this.RightChildNode.ToXmlFragment(doc, xmlns));
            return e;
        }

        public new static StalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            AndNode s = new AndNode
            {
                LeftChildNode = StalkNode.NewFromXmlFragment(xmlNode.ChildNodes[0]),
                RightChildNode = StalkNode.NewFromXmlFragment(xmlNode.ChildNodes[1])
            };
            return s;
        }

        public override string ToString()
        {
            return "(&:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}
