namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Castle.Core;
    using Castle.Core.Logging;
    using EyeInTheSky.Exceptions;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;

    public abstract class ConfigFileBase<T> : IInitializable 
        where T : class, INamedItem
    {
        private readonly string configurationFileName;
        private readonly object saveLock = new object();
        private readonly string configurationNodeName;
        private readonly Func<XmlElement, T> objectFactory;
        private readonly Func<T, XmlDocument, XmlElement> xmlFactory;
        private readonly IFileService fileService;

        protected readonly ILogger Logger;

        protected ConfigFileBase(
            string configurationFileName,
            string configurationNodeName,
            ILogger logger,
            Func<XmlElement, T> objectFactory,
            Func<T, XmlDocument, XmlElement> xmlFactory,
            IFileService fileService)
        {
            this.configurationFileName = configurationFileName;
            this.configurationNodeName = configurationNodeName;
            this.Logger = logger;
            this.objectFactory = objectFactory;
            this.xmlFactory = xmlFactory;
            this.fileService = fileService;
            this.ItemList = new SortedDictionary<string, T>();
            
            if (!this.fileService.FileExists(this.configurationFileName))
            {
                this.Logger.WarnFormat(
                    "Can't find stalk configuration file at {0}, using defaults",
                    this.configurationFileName);
                this.DoSave();
            }
        }

        protected SortedDictionary<string, T> ItemList { get; private set; }
        protected bool Initialised { get; private set; }
        
        public IReadOnlyList<T> Items
        {
            get
            {
                if (!this.Initialised)
                {
                    throw new ApplicationException("Cannot get configuration when not initialised!");
                }
                
                lock (this.ItemList)
                {
                    return new List<T>(this.ItemList.Values);
                }
            }
        }
        
        public T this[string itemName]
        {
            get
            {
                if (!this.Initialised)
                {
                    throw new ApplicationException("Cannot get configuration when not initialised!");
                }
                
                lock (this.ItemList)
                {
                    if (this.ItemList.ContainsKey(itemName))
                    {
                        return this.ItemList[itemName];
                    }
                }

                return null;
            }
        }

        public void Add(T item)
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot mutate configuration when not initialised!");
            }
            
            lock (this.ItemList)
            {
                this.ItemList.Add(item.Identifier, item);
            }
            
            this.OnAdd(item);
        }

        public void Remove(string key)
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot mutate configuration when not initialised!");
            }

            T item = null;
            
            lock (this.ItemList)
            {
                if (this.ItemList.ContainsKey(key))
                {
                    item = this.ItemList[key];
                    this.ItemList.Remove(key);
                }
            }

            if (item != null)
            {
                this.OnRemove(item);
            }
        }

        public bool ContainsKey(string stalkName)
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot get configuration when not initialised!");
            }
            
            lock (this.ItemList)
            {
                return this.ItemList.ContainsKey(stalkName);
            }
        }
       
        public void Initialize()
        {
            this.Logger.Info("Loading from configuration...");
            try
            {
                this.Initialised = false;

                var sr = new StreamReader(this.fileService.GetReadableStream(this.configurationFileName));

                var doc = new XmlDocument();
                doc.LoadXml(sr.ReadToEnd());
                var docElement = doc.DocumentElement;

                if (docElement == null)
                {
                    this.Logger.ErrorFormat("Invalid XML in {0} configuration file.", this.configurationFileName);
                    throw new ConfigurationException("Invalid XML in configuration file.");
                }

                if (!docElement.HasChildNodes)
                {
                    this.Logger.ErrorFormat("No list found in {0} configuration file.", this.configurationFileName);
                    throw new ConfigurationException("Invalid XML in configuration file.");
                }

                XmlElement configParent = null;
                foreach (XmlNode docChild in docElement.ChildNodes)
                {
                    var element = docChild as XmlElement;
                    if (element == null)
                    {
                        continue;
                    }

                    if (element.Name != this.configurationNodeName)
                    {
                        continue;
                    }
                    
                    configParent = element;
                }

                if (configParent == null)
                {
                    this.Logger.ErrorFormat("No list found in {0} configuration file.", this.configurationFileName);
                    throw new ConfigurationException("Invalid XML in configuration file.");
                }

                var loadedItems = new SortedList<string, T>();
                foreach (XmlNode configChild in configParent.ChildNodes)
                {
                    if (configChild.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    var configItem = this.objectFactory((XmlElement) configChild);
                    loadedItems.Add(configItem.Identifier, configItem);
                }

                lock (this.ItemList)
                {
                    this.ItemList.Clear();
                    
                    foreach (var i in loadedItems)
                    {
                        this.ItemList.Add(i.Key, i.Value);
                    }
                }

                sr.Close();

                this.DoSave();  
                
                this.Initialised = true;
                
                this.Logger.InfoFormat("Successfully loaded {0} from configuration.", loadedItems.Count);
            }
            catch (XmlException ex)
            {
                this.Logger.Error("Configuration file is invalid", ex);
                throw;
            }

            this.LocalInitialise();
        }
        
        protected virtual void LocalInitialise(){}
        protected virtual void OnAdd(T item){}
        protected virtual void OnRemove(T item){}

        public void Save()
        {
            if (!this.Initialised)
            {
                throw new ApplicationException("Cannot save configuration when not initialised!");
            }

            this.DoSave();
        }

        private void DoSave()
        {
            lock (this.saveLock)
            {
                SortedList<string, T> itemListClone;
                lock (this.ItemList)
                {
                    itemListClone = new SortedList<string, T>(this.ItemList);
                }

                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
                var root = doc.CreateElement("eyeinthesky");

                var configElement = doc.CreateElement(this.configurationNodeName);
                
                foreach (var kvp in itemListClone)
                {
                    configElement.AppendChild(this.xmlFactory(kvp.Value, doc));
                }

                root.AppendChild(configElement);

                doc.AppendChild(root);

                using (var writableStream = this.fileService.GetWritableStream(this.configurationFileName))
                {
                    doc.Save(writableStream);
                    writableStream.Flush();
                    writableStream.Close();
                }
            }
        }
    }
}