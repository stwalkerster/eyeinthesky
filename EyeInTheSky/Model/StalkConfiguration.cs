namespace EyeInTheSky.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public class StalkConfiguration
    {
        private const string XmlNamespace = "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd";
        
        private readonly string configurationFileName;
        private readonly ILogger logger;
        private readonly IStalkFactory stalkFactory;
        private readonly SortedList<string, IStalk> stalks = new SortedList<string, IStalk>();
        private readonly object stalkSaveLock = new object();
        
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
                this.DoSave();
            }
        }

        public IReadOnlyList<IStalk> StalkList
        {
            get
            {
                if (!this.initialised)
                {
                    throw new ApplicationException("Cannot get configuration when not initialised!");
                }
                
                lock (this.stalks)
                {
                    return new List<IStalk>(this.stalks.Values);
                }
            }
        }

        public IStalk this[string stalkName]
        {
            get
            {
                if (!this.initialised)
                {
                    throw new ApplicationException("Cannot get configuration when not initialised!");
                }
                
                lock (this.stalks)
                {
                    return this.stalks[stalkName];
                }
            }
        }

        public void Add(string key, IStalk stalk)
        {
            if (!this.initialised)
            {
                throw new ApplicationException("Cannot mutate configuration when not initialised!");
            }
            
            lock (this.stalks)
            {
                this.stalks.Add(key, stalk);
            }
        }

        public void Remove(string key)
        {
            if (!this.initialised)
            {
                throw new ApplicationException("Cannot mutate configuration when not initialised!");
            }
            
            lock (this.stalks)
            {
                this.stalks.Remove(key);
            }
        }

        public void Initialise()
        {
            this.logger.Info("Loading stalks from configuration...");
            try
            {
                this.initialised = false;
                
                var sr = new StreamReader(this.configurationFileName);

                var navigator = new XPathDocument(sr).CreateNavigator();

                var xnt = navigator.NameTable;
                var xnm = new XmlNamespaceManager(xnt);
                xnm.AddNamespace("isky", XmlNamespace);

                var stalknav = navigator.SelectSingleNode("//isky:stalks", xnm);
                
                int stalksCount;
                
                lock (this.stalks)
                {
                    this.stalks.Clear();
                    
                    if (stalknav != null)
                    {
                        var loadedStalks = this.LoadFromXmlFragment(stalknav.OuterXml, xnt);

                        foreach (var stalk in loadedStalks)
                        {
                            this.stalks.Add(stalk.Key, stalk.Value);
                        }
                    }
                    
                    stalksCount = this.stalks.Count;
                }

                sr.Close();

                this.DoSave();  
                
                this.initialised = true;
                
                this.logger.InfoFormat("Successfully loaded {0} stalks from configuration.", stalksCount);
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
            lock (this.stalkSaveLock)
            {
                SortedList<string, IStalk> stalkListClone;
                lock (this.stalks)
                {
                    stalkListClone = new SortedList<string, IStalk>(this.stalks);
                }

                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
                var root = doc.CreateElement("eyeinthesky", XmlNamespace);

                var stalksElement = doc.CreateElement("stalks", XmlNamespace);
                
                foreach (var kvp in stalkListClone)
                {
                    stalksElement.AppendChild(this.stalkFactory.ToXmlElement(kvp.Value, doc, XmlNamespace));
                }

                root.AppendChild(stalksElement);

                doc.AppendChild(root);

                doc.Save(this.configurationFileName);
            }
        }

        public IEnumerable<IStalk> MatchStalks(IRecentChange rc)
        {
            if (!this.initialised)
            {
                throw new ApplicationException("Cannot match when not initialised!");
            }
            
            SortedList<string, IStalk> stalkListClone;
            lock (this.stalks)
            {
                stalkListClone = new SortedList<string, IStalk>(this.stalks);
            }
            
            foreach (var s in stalkListClone)
            {
                bool isMatch;
             
                try
                {
                    isMatch = s.Value.Match(rc);
                }
                catch (InvalidOperationException ex)
                {
                    this.logger.ErrorFormat(ex, "Error during evaluation of stalk {0}", s.Key);
                    // skip this stalk, resume with the others
                    continue;
                }
                
                if (isMatch)
                {
                    yield return s.Value;
                }
            }
        }

        public bool ContainsKey(string stalkName)
        {
            if (!this.initialised)
            {
                throw new ApplicationException("Cannot get configuration when not initialised!");
            }
            
            lock (this.stalks)
            {
                return this.stalks.ContainsKey(stalkName);
            }
        }
    }
}