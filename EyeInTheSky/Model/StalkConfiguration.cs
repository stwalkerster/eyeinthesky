namespace EyeInTheSky.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using EyeInTheSky.Helpers.Interfaces;
    using EyeInTheSky.Model.Interfaces;

    public class StalkConfiguration
    {
        private const string XmlNamespace = "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd";
        
        private readonly string configurationFileName;
        private readonly ILogger logger;
        private readonly IStalkFactory stalkFactory;
        private SortedList<string, IStalk> stalks;

        private bool initialised;

        public StalkConfiguration(IAppConfiguration configuration, ILogger logger, IStalkFactory stalkFactory)
        {
            this.configurationFileName = configuration.StalkConfigFile;
            this.logger = logger;
            this.stalkFactory = stalkFactory;

            if (!new FileInfo(this.configurationFileName).Exists)
            {
                this.logger.Warn(
                    "Can't find stalk configuration file at " + this.configurationFileName
                                                          + ", using defaults");
                this.stalks = new SortedList<string, IStalk>();
                this.DoSave();
            }
        }

        public SortedList<string, IStalk> Stalks
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
                this.stalks = stalknav != null ? this.LoadFromXmlFragment(stalknav.OuterXml, xnt) : new SortedList<string, IStalk>();

                sr.Close();

                this.initialised = true;
                this.logger.InfoFormat("Successfully loaded {0} stalks from configuration.", this.stalks.Count);
            }
            catch (XmlException ex)
            {
                this.logger.Error("Configuration file is invalid", ex);
                throw;
            }
        }

        private SortedList<string, IStalk> LoadFromXmlFragment(string fragment, XmlNameTable nameTable)
        {
            var list = new SortedList<string, IStalk>();

            var xd = new XmlDocument(nameTable);
            xd.LoadXml(fragment);
            var node = xd.ChildNodes[0];
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                var element = (XmlElement) childNode;

                var stalk = (ComplexStalk) this.stalkFactory.NewFromXmlElement(element);
                list.Add(stalk.Flag, stalk);
            }

            return list;
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
                stalksElement.AppendChild(this.stalkFactory.ToXmlElement(kvp.Value, doc, XmlNamespace));
            }

            root.AppendChild(stalksElement);

            doc.AppendChild(root);

            doc.Save(this.configurationFileName);
        }

        public IEnumerable<IStalk> MatchStalks(IRecentChange rc)
        {
            foreach (var s in this.stalks)
            {
                if(s.Value.Match(rc))
                {
                    yield return s.Value;
                }
            }
        }
    }
}