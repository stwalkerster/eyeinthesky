namespace EyeInTheSky.StalkNodes
{
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;

    class FalseNode : LogicalNode
    {
        public override bool Match(IRecentChange rc)
        {
            return false;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("false",xmlns);
        }

        public new static StalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            return new FalseNode();
        }

        public override string ToString()
        {
            return "(false)";
        }
    }
}
