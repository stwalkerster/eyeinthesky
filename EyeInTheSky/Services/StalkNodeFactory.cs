namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.ExternalProviders.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class StalkNodeFactory : IStalkNodeFactory
    {
        private readonly IPhabricatorExternalProvider phabricatorExternalProvider;

        public StalkNodeFactory(IPhabricatorExternalProvider phabricatorExternalProvider)
        {
            this.phabricatorExternalProvider = phabricatorExternalProvider;
        }
        
        public IStalkNode NewFromXmlFragment(XmlElement fragment)
        {
            switch (fragment.Name)
            {
                case "and":
                case "or":
                case "x-of":
                    return this.NewMultiChildNode(fragment);
                
                case "xor":
                    return this.NewDoubleChildNode(fragment);

                case "not":
                    return this.NewSingleChildNode(fragment);

                case "user":
                case "page":
                case "summary":
                case "flag":
                case "usergroup":
                case "incategory":
                case "log":
                    return this.NewLeafNode(fragment);

                case "true":
                    return new TrueNode();
                case "false":
                    return new FalseNode();

                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }
        }

        private IStalkNode NewSingleChildNode(XmlElement fragment)
        {
            SingleChildLogicalNode node;
            switch (fragment.Name)
            {
                case "external":
                    return NewExternalNode(fragment);
                case "not":
                    node = new NotNode();
                    break;    
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            var child = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[0]);

            node.ChildNode = child;

            return node;
        }

        private IStalkNode NewExternalNode(XmlElement fragment)
        {
            var extNode = new ExternalNode();

            if (fragment.Attributes["provider"] != null)
            {
                extNode.Provider = fragment.Attributes["provider"].Value;
            }

            if (fragment.Attributes["location"] != null)
            {
                extNode.Location = fragment.Attributes["location"].Value;
            }

            if (extNode.Provider == null || extNode.Location == null)
            {
                throw new XmlException("Could not determine location of external XML fragment");
            }

            IExternalProvider provider;
            switch (extNode.Provider)
            {
                    
                case "phabricator":
                    provider = this.phabricatorExternalProvider;
                    break;
                default:
                    throw new XmlException("Unknown external handler type");
            }

            extNode.ChildNode = this.NewFromXmlFragment(provider.GetFragmentFromSource(extNode.Location));
            return extNode;
        }

        private IStalkNode NewDoubleChildNode(XmlElement fragment)
        {
            DoubleChildLogicalNode node;
            switch (fragment.Name)
            {
                case "xor":
                    node = new XorNode();
                    break;
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            var left = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[0]);
            var right = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[1]);

            node.LeftChildNode = left;
            node.RightChildNode = right;

            return node;
        }

        private IStalkNode NewMultiChildNode(XmlElement fragment)
        {
            MultiChildLogicalNode node;
            switch (fragment.Name)
            {
                case "and":
                    node = new AndNode();
                    break;
                case "or":
                    node = new OrNode();
                    break;
                case "x-of":
                    var xofnode = new XOfStalkNode();
                    var minAttr = fragment.Attributes["minimum"];
                    if (minAttr != null)
                    {
                        xofnode.Minimum = XmlConvert.ToInt32(minAttr.Value);
                    }

                    var maxAttr = fragment.Attributes["maximum"];
                    if (maxAttr != null)
                    {
                        xofnode.Maximum = XmlConvert.ToInt32(maxAttr.Value);
                    }

                    node = xofnode;
                    break;
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            node.ChildNodes = new List<IStalkNode>();
            
            foreach (XmlNode fragmentChildNode in fragment.ChildNodes)
            {
                var elem = fragmentChildNode as XmlElement;
                if (elem == null)
                {
                    continue;
                }

                var n = this.NewFromXmlFragment(elem);
                node.ChildNodes.Add(n);
            }

            return node;
        }

        private IStalkNode NewLeafNode(XmlElement fragment)
        {
            LeafNode node;
            switch (fragment.Name)
            {
                case "user":
                case "page":
                case "summary":
                case "flag":
                case "log":
                    return this.NewRegexLeafNode(fragment);
                case "usergroup":
                    node = new UserGroupStalkNode();
                    break;
                case "incategory":
                    node = new InCategoryNode();
                    break;
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            node.SetMatchExpression(fragment.Attributes["value"].Value);

            return node;
        }

        private IStalkNode NewRegexLeafNode(XmlElement fragment)
        {
            RegexLeafNode node;
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
                case "log":
                    node = new LogStalkNode();
                    break;
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            if (fragment.Attributes["caseinsensitive"] != null)
            {
                node.CaseInsensitive = XmlConvert.ToBoolean(fragment.Attributes["caseinsensitive"].Value);
            }
            
            node.SetMatchExpression(fragment.Attributes["value"].Value);

            return node;
        }

        public XmlElement ToXml(XmlDocument doc, IStalkNode node)
        {
            var logicalNode = node as LogicalNode;
            if (logicalNode != null)
            {
                return this.LogicalToXml(doc, logicalNode);
            }

            var leafNode = node as LeafNode;
            if (leafNode != null)
            {
                return this.LeafToXml(doc, leafNode);
            }

            throw new XmlException();
        }

        private XmlElement CreateElement(XmlDocument doc, IStalkNode node)
        {
            var attr =
                node.GetType().GetCustomAttributes(typeof(StalkNodeTypeAttribute), false).FirstOrDefault() as
                    StalkNodeTypeAttribute;

            if (attr == null)
            {
                throw new XmlException("Unable to find XML element type during serialisation");
            }

            var elem = doc.CreateElement(attr.ElementName);
            return elem;
        }

        private XmlElement LeafToXml(XmlDocument doc, LeafNode node)
        {
            var elem = this.CreateElement(doc, node);
            elem.SetAttribute("value", node.GetMatchExpression());
            
            var regexLeafNode = node as RegexLeafNode;
            if (regexLeafNode != null)
            {
                if (regexLeafNode.CaseInsensitive)
                {
                    elem.SetAttribute("caseinsensitive", XmlConvert.ToString(regexLeafNode.CaseInsensitive));
                }
            }
            
            return elem;
        }

        private XmlElement LogicalToXml(XmlDocument doc, LogicalNode node)
        {
            var doubleChildLogicalNode = node as DoubleChildLogicalNode;
            if (doubleChildLogicalNode != null)
            {
                return this.DoubleChildToXml(doc, doubleChildLogicalNode);
            }
            
            var multiChildLogicalNode = node as MultiChildLogicalNode;
            if (multiChildLogicalNode != null)
            {
                return this.MultiChildToXml(doc, multiChildLogicalNode);
            }

            var singleChildLogicalNode = node as SingleChildLogicalNode;
            if (singleChildLogicalNode != null)
            {
                return this.SingleChildToXml(doc, singleChildLogicalNode);
            }   

            return this.CreateElement(doc, node);
        }

        private XmlElement SingleChildToXml(XmlDocument doc, SingleChildLogicalNode node)
        {
            var externalNode = node as ExternalNode;
            if (externalNode != null)
            {
                return this.ExternalNodeToXml(doc, externalNode);
            } 
            
            var elem = this.CreateElement(doc, node);

            elem.AppendChild(this.ToXml(doc, node.ChildNode));

            return elem;
        }

        private XmlElement ExternalNodeToXml(XmlDocument doc, ExternalNode node)
        {
            var elem = this.CreateElement(doc, node);
            elem.SetAttribute("provider", node.Provider);
            elem.SetAttribute("location", node.Location);

            return elem;
        }

        private XmlElement DoubleChildToXml(XmlDocument doc, DoubleChildLogicalNode node)
        {
            var elem = this.CreateElement(doc, node);

            elem.AppendChild(this.ToXml(doc, node.LeftChildNode));
            elem.AppendChild(this.ToXml(doc, node.RightChildNode));

            return elem;
        }

        private XmlElement MultiChildToXml(XmlDocument doc, MultiChildLogicalNode node)
        {
            var xofNode = node as XOfStalkNode;
            if (xofNode != null)
            {
                return this.XOfToXml(doc, xofNode);
            }
           
            var elem = this.CreateElement(doc, node);

            foreach (var childNode in node.ChildNodes)
            {
                elem.AppendChild(this.ToXml(doc, childNode));    
            }
            
            return elem;
        }

        private XmlElement XOfToXml(XmlDocument doc, XOfStalkNode node)
        {
            var elem = this.CreateElement(doc, node);

            if (node.Minimum.HasValue)
            {
                elem.SetAttribute("minimum", XmlConvert.ToString(node.Minimum.Value));
            }

            if (node.Maximum.HasValue)
            {
                elem.SetAttribute("maximum", XmlConvert.ToString(node.Maximum.Value));
            }
            
            foreach (var childNode in node.ChildNodes)
            {
                elem.AppendChild(this.ToXml(doc, childNode));
            }

            return elem;
        }
    }
}