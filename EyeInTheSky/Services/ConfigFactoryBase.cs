namespace EyeInTheSky.Services
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;

    public abstract class ConfigFactoryBase
    {
        protected ConfigFactoryBase(ILogger logger, IStalkNodeFactory stalkNodeFactory)
        {
            this.Logger = logger;
            this.StalkNodeFactory = stalkNodeFactory;
        }
        
        protected ILogger Logger { get; private set; }
        protected IStalkNodeFactory StalkNodeFactory { get; set; }

        internal DateTime ParseDate(string flagName, string input, string propName)
        {
            DateTime result;

            try
            {
                result = XmlConvert.ToDateTime(input, XmlDateTimeSerializationMode.Utc);
            }
            catch (FormatException ex)
            {
                this.Logger.WarnFormat("Unknown date format in stalk '{0}' {2}: {1}", flagName, input, propName);

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    var err = string.Format("Failed date parse for stalk '{0}' {2}: {1}", flagName, input, propName);

                    this.Logger.Error(err);
                    throw new FormatException(err, ex);
                }
            }

            return result;
        }

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