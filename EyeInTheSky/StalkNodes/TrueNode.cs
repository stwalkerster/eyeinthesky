using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    class TrueNode : LogicalNode
    {
        public override bool Match(IRecentChange rc)
        {
            return true;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("true", xmlns);
        }

        public new static StalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            return new TrueNode();
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}
