namespace EyeInTheSky.Model
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;

    public class StalkConfiguration
    {
        private const string XmlNamespace = "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd";
        
        private readonly string configurationFileName;
        private readonly ILogger logger;
        private StalkList stalks;

        private bool initialised;

        public StalkConfiguration(IAppConfiguration configuration, ILogger logger)
        {
            this.configurationFileName = configuration.StalkConfigFile;
            this.logger = logger;

            if (!new FileInfo(this.configurationFileName).Exists)
            {
                this.logger.Warn(
                    "Can't find stalk configuration file at " + this.configurationFileName
                                                          + ", using defaults");
                this.stalks = new StalkList();
                this.DoSave();
            }
        }

        public StalkList Stalks
        {
            get
            {
                if (!this.initialised)
                {
                    throw new ApplicationException("Cannot get configuration when not initialised!");
                }

                return this.stalks;
            }
        }

        public void Initialise()
        {
            this.logger.Info("Loading stalks from configuration...");
            try
            {
                var sr = new StreamReader(this.configurationFileName);

                var navigator = new XPathDocument(sr).CreateNavigator();

                var xnt = navigator.NameTable;
                var xnm = new XmlNamespaceManager(xnt);
                xnm.AddNamespace("isky", XmlNamespace);

                var stalknav = navigator.SelectSingleNode("//isky:stalks", xnm);
                this.stalks = stalknav != null ? StalkList.LoadFromXmlFragment(stalknav.OuterXml, xnt) : new StalkList();

                sr.Close();

                this.initialised = true;
                this.logger.Info("Successfully loaded stalks from configuration.");
            }
            catch (XmlException ex)
            {
                this.logger.Error("Configuration file is invalid", ex);
                throw;
            }
        }

        public void Save()
        {
            if (!this.initialised)
            {
                throw new ApplicationException("Cannot save configuration when not initialised!");
            }

            this.DoSave();
        }

        private void DoSave()
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            var root = doc.CreateElement("eyeinthesky", XmlNamespace);

            var stalksElement = doc.CreateElement("stalks", XmlNamespace);
            foreach (var kvp in this.stalks)
            {
                stalksElement.AppendChild(kvp.Value.ToXmlFragment(doc, XmlNamespace));
            }

            root.AppendChild(stalksElement);

            doc.AppendChild(root);

            doc.Save(this.configurationFileName);
        }
    }
}