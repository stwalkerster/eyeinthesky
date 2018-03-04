using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace EyeInTheSky
{
    using Castle.Core.Logging;

    public class StalkConfiguration
    {
        private readonly string configurationFileName;
        private readonly ILogger logger;
        private StalkList stalks;

        private bool initialised;

        public StalkConfiguration(AppConfiguration configuration, ILogger logger)
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

            this.Initialise();
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

        private void Initialise()
        {
            this.logger.Info("Loading stalks from configuration...");
            try
            {
                StreamReader sr = new StreamReader(this.configurationFileName);

                XPathDocument xPathDocument = new XPathDocument(sr);
                XPathNavigator navigator = xPathDocument.CreateNavigator();

                XmlNameTable xnt = navigator.NameTable;
                XmlNamespaceManager xnm = new XmlNamespaceManager(xnt);
                xnm.AddNamespace(
                    "isky",
                    "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd");

                XPathNavigator stalknav = navigator.SelectSingleNode("//isky:stalks", xnm);
                this.stalks = StalkList.fetch(stalknav, xnm);

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
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            string xmlns = "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd";
            XmlElement root = doc.CreateElement("eyeinthesky", xmlns);

            XmlElement stalkselem = doc.CreateElement("stalks", xmlns);
            foreach (KeyValuePair<string, ComplexStalk> kvp in this.stalks)
            {
                stalkselem.AppendChild(kvp.Value.ToXmlFragment(doc, xmlns));
            }

            root.AppendChild(stalkselem);

            doc.AppendChild(root);

            doc.Save(this.configurationFileName);
        }
    }
}