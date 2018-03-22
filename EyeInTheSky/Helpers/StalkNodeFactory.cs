﻿namespace EyeInTheSky.Helpers
{
    using System.Linq;
    using System.Xml;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.StalkNodes;

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
                case "usergroup":
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

            var left = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[0]);
            var right = this.NewFromXmlFragment((XmlElement) fragment.ChildNodes[1]);

            node.LeftChildNode = left;
            node.RightChildNode = right;

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
            var logicalNode = node as DoubleChildLogicalNode;
            if (logicalNode != null)
            {
                return this.DoubleChildToXml(doc, xmlns, logicalNode);
            }

            var childLogicalNode = node as SingleChildLogicalNode;
            if (childLogicalNode != null)
            {
                return this.SingleChildToXml(doc, xmlns, childLogicalNode);
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
    }
}