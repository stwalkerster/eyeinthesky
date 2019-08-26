namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Model.StalkNodes.NumericNodes;
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
            IStalkNode node;
            
            switch (fragment.Name)
            {
                case "and":
                case "or":
                case "x-of":
                    node = this.NewMultiChildNode(fragment);
                    break;
                
                case "xor":
                    node = this.NewDoubleChildNode(fragment);
                    break;

                case "not":
                case "external": 
                case "expiry":
                    node = this.NewSingleChildNode(fragment);
                    break;

                case "user":
                case "page":
                case "summary":
                case "flag":
                case "usergroup":
                case "incategory":
                case "log":
                    node = this.NewLeafNode(fragment);
                    break;
                
                case "infixnumeric":
                    node = this.NewInfixNumeric(fragment);
                    break;

                case "true":
                    node = new TrueNode();
                    break;
                case "false":
                    node = new FalseNode();
                    break;

                
                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }

            var fragmentAttribute = fragment.Attributes["comment"];
            if (fragmentAttribute != null)
            {
                node.Comment = fragmentAttribute.Value;
            }
            
            return node;
        }
        
        public INumberProviderNode NewNumericFromXmlFragment(XmlElement fragment)
        {
            switch (fragment.Name)
            {
                case "number":
                    var node = new StaticNumberNode();
                    var fragmentAttribute = fragment.Attributes["value"];
                    if (fragmentAttribute != null)
                    {
                        node.Value = XmlConvert.ToInt64(fragmentAttribute.Value);
                    }

                    return node;
                case "diffsize":
                    return new DiffDeltaNumberNode();
                case "pagesize":
                    return new PageSizeNumberNode();

                default:
                    throw new XmlException("Unknown element " + fragment.Name);
            }
        }

        private IStalkNode NewInfixNumeric(XmlElement fragment)
        {
            var node = new InfixNumericLogicalNode();
            node.Operator = fragment.Attributes["operator"].Value;

            var lcn = this.NewNumericFromXmlFragment((XmlElement) fragment.ChildNodes[0]);
            var rcn = this.NewNumericFromXmlFragment((XmlElement) fragment.ChildNodes[1]);

            node.LeftChildNode = lcn;
            node.RightChildNode = rcn;

            return node;
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
                case "expiry":
                    node = new ExpiryNode();

                    var timestampAttribute = fragment.Attributes["expiry"];
                    if (timestampAttribute != null)
                    {
                        ((ExpiryNode)node).Expiry = XmlConvert.ToDateTime(timestampAttribute.Value, XmlDateTimeSerializationMode.Utc);
                    }

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

            extNode.ChildNode = this.NewFromXmlFragment(provider.PopulateFromExternalSource(extNode));
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
                case "targetuser":
                    node = new TargetUserStalkNode();
                    break;                
                case "actinguser":
                    node = new ActingUserStalkNode();
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
            XmlElement element;
            
            var logicalNode = node as LogicalNode;
            if (logicalNode != null)
            {
                element = this.LogicalToXml(doc, logicalNode);
            }
            else
            {
                var leafNode = node as LeafNode;
                if (leafNode != null)
                {
                    element = this.LeafToXml(doc, leafNode);
                }
                else
                {
                    throw new XmlException();
                }
            }

            if (!string.IsNullOrWhiteSpace(node.Comment))
            {
                element.SetAttribute("comment", node.Comment);
            }
            
            return element;
        }

        private XmlElement CreateElement(XmlDocument doc, ITreeNode node)
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

            var infixNumeric = node as InfixNumericLogicalNode;
            if (infixNumeric != null)
            {
                return this.InfixNumericToXml(doc, infixNumeric);
            }
            
            return this.CreateElement(doc, node);
        }

        private XmlElement InfixNumericToXml(XmlDocument doc, InfixNumericLogicalNode node)
        {
            var elem = this.CreateElement(doc, node);

            elem.AppendChild(this.NumericToXml(doc, node.LeftChildNode));
            elem.AppendChild(this.NumericToXml(doc, node.RightChildNode));
            elem.SetAttribute("operator", node.Operator);

            return elem;
        }

        private XmlNode NumericToXml(XmlDocument doc, INumberProviderNode node)
        {
            if (node is DiffDeltaNumberNode || node is PageSizeNumberNode)
            {
                return this.CreateElement(doc, node);
            }

            var staticNumberNode = node as StaticNumberNode;
            if (staticNumberNode != null)
            {
                var elem = this.CreateElement(doc, node);
                elem.SetAttribute("value", XmlConvert.ToString(staticNumberNode.Value));
                return elem;
            }
            
            throw new XmlException();
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

            var expiryNode = node as ExpiryNode;
            if (expiryNode != null)
            {
                elem.SetAttribute("expiry", XmlConvert.ToString(expiryNode.Expiry, XmlDateTimeSerializationMode.Utc));
            }

            return elem;
        }

        private XmlElement ExternalNodeToXml(XmlDocument doc, ExternalNode node)
        {
            var elem = this.CreateElement(doc, node);
            elem.SetAttribute("provider", node.Provider);
            elem.SetAttribute("location", node.Location);
            elem.SetAttribute("comment", node.Comment);

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