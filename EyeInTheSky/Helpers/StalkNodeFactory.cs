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
                case "or":
                case "xor":
                    return this.NewDoubleChildNode(fragment);
                    
                case "not":
                    return this.NewSingleChildNode(fragment);
                    
                case "user":
                case "page":
                case "summary":
                case "flag":
                    return this.NewLeafNode(fragment);
                    
                case "true":
                    return new TrueNode();
                case "false":
                    return new FalseNode();
                    
                default:
                    throw new XmlException();
            }
        }
        
        private IStalkNode NewSingleChildNode(XmlElement fragment) {
            SingleChildLogicalNode node;
            switch (fragment.Name)
            {
                case "not":
                    node = new NotNode();
                    break;
                default:
                    throw new XmlException();
            }

            var child = this.NewFromXmlFragment((XmlElement)fragment.ChildNodes[0]);

            node.ChildNode = child;

            return node;
        }
        
        private IStalkNode NewDoubleChildNode(XmlElement fragment) {
            DoubleChildLogicalNode node;
            switch (fragment.Name)
            {
                case "and":
                    node = new AndNode();
                    break;
                case "or":
                    node = new OrNode();
                    break;
                case "xor":
                    node = new XorNode();
                    break;
                default:
                    throw new XmlException();
            }

            var left = this.NewFromXmlFragment((XmlElement)fragment.ChildNodes[0]);
            var right = this.NewFromXmlFragment((XmlElement)fragment.ChildNodes[1]);

            node.LeftChildNode = left;
            node.RightChildNode = right;

            return node;
        }
        
        private IStalkNode NewLeafNode(XmlElement fragment) {
            LeafNode node;
            switch (fragment.Name)
            {
                case "user":
                    node = new UserStalkNode();
                    break;
                case "page":
                    node = new PageStalkNode();
                    break;
                case "summary":
                    node = new SummaryStalkNode();
                    break;
                case "flag":
                    node = new FlagStalkNode();
                    break;
                default:
                    throw new XmlException();
            }

            node.SetMatchExpression(fragment.Attributes["value"].Value);

            return node;
        }
    }
}