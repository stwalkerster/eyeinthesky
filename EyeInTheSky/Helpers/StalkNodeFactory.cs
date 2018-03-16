using System.Xml;
using EyeInTheSky.Helpers.Interfaces;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Helpers
{
    public class StalkNodeFactory : IStalkNodeFactory
    {
        public IStalkNode NewFromXmlFragment(XmlElement fragment)
        {
            switch (fragment.Name)
            {
                case "and":
                    return AndNode.NewFromXmlFragment(fragment);
                case "or":
                    return OrNode.NewFromXmlFragment(fragment);
                case "not":
                    return NotNode.NewFromXmlFragment(fragment);
                case "xor":
                    return XorNode.NewFromXmlFragment(fragment);
                case "true":
                    return TrueNode.NewFromXmlFragment(fragment);
                case "false":
                    return FalseNode.NewFromXmlFragment(fragment);
                case "user":
                    return UserStalkNode.NewFromXmlFragment(fragment);
                case "page":
                    return PageStalkNode.NewFromXmlFragment(fragment);
                case "summary":
                    return SummaryStalkNode.NewFromXmlFragment(fragment);
                case "flag":
                    return FlagStalkNode.NewFromXmlFragment(fragment);
                default:
                    throw new XmlException();
            }
        }
    }
}