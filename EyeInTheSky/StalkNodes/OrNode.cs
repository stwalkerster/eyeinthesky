using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class OrNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.LeftChildNode.Match(rc) || this.RightChildNode.Match(rc);
        }

        public new static IStalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            var s = new OrNode
            {
                LeftChildNode = StalkNode.NewFromXmlFragment(xmlNode.ChildNodes[0]),
                RightChildNode = StalkNode.NewFromXmlFragment(xmlNode.ChildNodes[1])
            };
            return s;
        }
        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            var e = doc.CreateElement("or", xmlns);
            e.AppendChild(this.LeftChildNode.ToXmlFragment(doc, xmlns));
            e.AppendChild(this.RightChildNode.ToXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "(|:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}