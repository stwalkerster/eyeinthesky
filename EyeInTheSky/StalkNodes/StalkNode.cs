using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    public abstract class StalkNode : IStalkNode
    {
        public abstract bool Match(IRecentChange rc);

        public static IStalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "and":
                    return AndNode.NewFromXmlFragment(xmlNode);
                case "or":
                    return OrNode.NewFromXmlFragment(xmlNode);
                case "not":
                    return NotNode.NewFromXmlFragment(xmlNode);
                case "xor":
                    return XorNode.NewFromXmlFragment(xmlNode);
                case "true":
                    return TrueNode.NewFromXmlFragment(xmlNode);
                case "false":
                    return FalseNode.NewFromXmlFragment(xmlNode);
                case "user":
                    return UserStalkNode.NewFromXmlFragment(xmlNode);
                case "page":
                    return PageStalkNode.NewFromXmlFragment(xmlNode);
                case "summary":
                    return SummaryStalkNode.NewFromXmlFragment(xmlNode);
                case "flag":
                    return FlagStalkNode.NewFromXmlFragment(xmlNode);
                default:
                    throw new XmlException();
            }
        }

        public abstract XmlElement ToXmlFragment(XmlDocument doc, string xmlns);
    }
}
