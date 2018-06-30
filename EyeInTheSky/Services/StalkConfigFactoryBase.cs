namespace EyeInTheSky.Services
{
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;

    public abstract class StalkConfigFactoryBase : ConfigFactoryBase
    {
        protected StalkConfigFactoryBase(ILogger logger, IStalkNodeFactory stalkNodeFactory) : base(logger)
        {
            
            this.StalkNodeFactory = stalkNodeFactory;
        }

        protected IStalkNodeFactory StalkNodeFactory { get; set; }

        protected IStalkNode GetStalkTreeFromXml(XmlElement element, string flag)
        {
            IStalkNode baseNode = null;

            if (element.HasChildNodes)
            {
                var childNodeCollection = element.ChildNodes;

                foreach (XmlNode node in childNodeCollection)
                {
                    var xmlElement = node as XmlElement;
                    if (xmlElement == null)
                    {
                        continue;
                    }

                    if (xmlElement.Name == "searchtree")
                    {
                        baseNode = this.StalkNodeFactory.NewFromXmlFragment((XmlElement) xmlElement.FirstChild);
                        continue;
                    }

                    this.Logger.DebugFormat("Unrecognised child {0} of stalk {1}", xmlElement.Name, flag);
                }

                if (baseNode == null)
                {
                    this.Logger.InfoFormat("Assuming stalk {0} is legacy", flag);
                    baseNode = this.StalkNodeFactory.NewFromXmlFragment((XmlElement) element.FirstChild);
                }
            }

            return baseNode;
        }
    }
}