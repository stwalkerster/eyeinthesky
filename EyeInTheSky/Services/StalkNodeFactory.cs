﻿namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.StalkNodes;

    public class StalkNodeFactory : IStalkNodeFactory
    {
        public IStalkNode NewFromXmlFragment(XmlElement fragment)
        {
            switch (fragment.Name)
            {
                case "and":
                case "or":
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
                    return this.NewLeafNode(fragment);

                case "true":
                    return new TrueNode();
                case "false":
                    return new FalseNode();

                default:
                    throw new XmlException();
            }
        }

        private IStalkNode NewSingleChildNode(XmlElement fragment)
        {
            SingleChildLogicalNode node;
            switch (fragment.Name)
            {
                case "not":
                    node = new NotNode();
                    break;
                default:
                    throw new XmlException();
            }

            var child = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[0]);

            node.ChildNode = child;

            return node;
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
                    throw new XmlException();
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
                default:
                    throw new XmlException();
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
                    return this.NewRegexLeafNode(fragment);
                case "usergroup":
                    node = new UserGroupStalkNode();
                    break;
                case "incategory":
                    node = new InCategoryNode();
                    break;
                default:
                    throw new XmlException();
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
                default:
                    throw new XmlException();
            }

            node.SetMatchExpression(fragment.Attributes["value"].Value);

            return node;
        }

        public XmlElement ToXml(XmlDocument doc, string xmlns, IStalkNode node)
        {
            var logicalNode = node as LogicalNode;
            if (logicalNode != null)
            {
                return this.LogicalToXml(doc, xmlns, logicalNode);
            }

            var leafNode = node as LeafNode;
            if (leafNode != null)
            {
                return this.LeafToXml(doc, xmlns, leafNode);
            }

            throw new XmlException();
        }

        private XmlElement CreateElement(XmlDocument doc, string xmlns, IStalkNode node)
        {
            var attr =
                node.GetType().GetCustomAttributes(typeof(StalkNodeTypeAttribute), false).FirstOrDefault() as
                    StalkNodeTypeAttribute;

            if (attr == null)
            {
                throw new XmlException("Unable to find XML element type during serialisation");
            }

            var elem = doc.CreateElement(attr.ElementName, xmlns);
            return elem;
        }

        private XmlElement LeafToXml(XmlDocument doc, string xmlns, LeafNode node)
        {
            var elem = this.CreateElement(doc, xmlns, node);
            elem.SetAttribute("value", node.GetMatchExpression());

            return elem;
        }

        private XmlElement LogicalToXml(XmlDocument doc, string xmlns, LogicalNode node)
        {
            var doubleChildLogicalNode = node as DoubleChildLogicalNode;
            if (doubleChildLogicalNode != null)
            {
                return this.DoubleChildToXml(doc, xmlns, doubleChildLogicalNode);
            }
            
            var multiChildLogicalNode = node as MultiChildLogicalNode;
            if (multiChildLogicalNode != null)
            {
                return this.MultiChildToXml(doc, xmlns, multiChildLogicalNode);
            }

            var singleChildLogicalNode = node as SingleChildLogicalNode;
            if (singleChildLogicalNode != null)
            {
                return this.SingleChildToXml(doc, xmlns, singleChildLogicalNode);
            }   

            return this.CreateElement(doc, xmlns, node);
        }

        private XmlElement SingleChildToXml(XmlDocument doc, string xmlns, SingleChildLogicalNode node)
        {
            var elem = this.CreateElement(doc, xmlns, node);

            elem.AppendChild(this.ToXml(doc, xmlns, node.ChildNode));

            return elem;
        }

        private XmlElement DoubleChildToXml(XmlDocument doc, string xmlns, DoubleChildLogicalNode node)
        {
            var elem = this.CreateElement(doc, xmlns, node);

            elem.AppendChild(this.ToXml(doc, xmlns, node.LeftChildNode));
            elem.AppendChild(this.ToXml(doc, xmlns, node.RightChildNode));

            return elem;
        }

        private XmlElement MultiChildToXml(XmlDocument doc, string xmlns, MultiChildLogicalNode node)
        {
            var elem = this.CreateElement(doc, xmlns, node);

            foreach (var childNode in node.ChildNodes)
            {
                elem.AppendChild(this.ToXml(doc, xmlns, childNode));    
            }
            
            return elem;
        }
    }
}