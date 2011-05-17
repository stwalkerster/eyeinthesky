using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace EyeInTheSky
{
    public class Configuration
    {
        private readonly string configurationFileName;
        private SortedList<string, string> _configuration;
        private StalkList stalks;
        private SortedList<string,AccessListEntry> userlist;
        private List<StalkLogItem> stalklog;

        public void LogStalkTrigger(string s, RecentChange rc)
        {
            StalkLogItem sli = new StalkLogItem(s,rc);
            stalklog.Add(sli);
            this.save();
        }

        public Configuration(string fileName)
        {
            configurationFileName = fileName;

            if(!initialise())
            {
                Logger.instance().addToLog("Configuration initialisation failed. Retrying...", Logger.LogTypes.Error);
                if (!initialise())
                {
                    throw  new ArgumentException();
                }
            }
        }

        public string this[string configOption]
        {
            get
            {
                if (_configuration.ContainsKey(configOption))
                {
                    return _configuration[configOption];
                }
                throw new ArgumentOutOfRangeException(configOption);
            }
            set
            {
                if (_configuration.ContainsKey(configOption))
                {
                    _configuration[configOption] = value;
                }
                else
                {
                    _configuration.Add(configOption, value);
                }
                this.save();
            }
        }

        public string this[string configOption, string defaultValue]
        {
            get
            {
                if (_configuration.ContainsKey(configOption))
                {
                    return _configuration[configOption];
                }

                this[configOption] = defaultValue;
                return defaultValue;
            }
            set
            {
                this[configOption] = value;
            }
        }

        public StalkList Stalks
        {
            get { return stalks; }
        }

        public SortedList<string,AccessListEntry> accessList
        {
            get { return userlist; }
        }

        public void delete(string configOption)
        {
            _configuration.Remove(configOption);
        }

        public bool rehash()
        {
            return initialise();
        }

        private bool initialise()
        {
            if (!new FileInfo(configurationFileName).Exists)
            {
                new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(5) + "/DefaultConfiguration.xml").CopyTo(configurationFileName);
                throw new Exception("Please open the configuration file and define required values");
            }

            try
            {
                StreamReader sr = new StreamReader(configurationFileName);

                XPathDocument xPathDocument = new XPathDocument(sr);
                XPathNavigator navigator = xPathDocument.CreateNavigator();

                XmlNameTable xnt = navigator.NameTable;
                XmlNamespaceManager xnm = new XmlNamespaceManager(xnt);
                xnm.AddNamespace("isky",
                                 "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd");

                XPathNodeIterator xpni = navigator.Select("//isky:option", xnm);

                _configuration = new SortedList<string, string>();
                while (xpni.MoveNext())
                {
                    _configuration.Add(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("value", ""));
                }

                XPathNavigator stalknav = navigator.SelectSingleNode("//isky:stalks", xnm);
                stalks = StalkList.fetch(stalknav, xnm);

                userlist = new SortedList<string,AccessListEntry>();
                xpni = navigator.Select("//isky:users/isky:user", xnm);
                while (xpni.MoveNext())
                {
                    string access = xpni.Current.GetAttribute("access", "");
                    User.UserRights level = (User.UserRights) Enum.Parse(typeof (User.UserRights), access);
                    AccessListEntry u = new AccessListEntry(xpni.Current.GetAttribute("hostmask", ""),level);
                    accessList.Add(u.HostnameMask,u);
                }

                stalklog = new List<StalkLogItem>();
                XPathNavigator nav = navigator.SelectSingleNode("//isky:stalklog", xnm);

                XmlDocument xd = new XmlDocument(nav.NameTable);
                xd.LoadXml(nav.OuterXml);
                XmlNode node = xd.ChildNodes[0];
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType != XmlNodeType.Element)
                        continue;

                    XmlElement element = (XmlElement)childNode;

                    StalkLogItem sli = StalkLogItem.newFromXmlElement(element);
                    stalklog.Add(sli);
                }
                

                sr.Close();
                return true;
            }
            catch (XmlException ex)
            {
                GlobalFunctions.errorLog(ex);
                Logger.instance().addToLog("Deleting corrupt configuration file...", Logger.LogTypes.Error);
                new FileInfo(configurationFileName).Delete();
                return false;
            }
        }

        public void save()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
            string xmlns = "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd";
            XmlElement root = doc.CreateElement("eyeinthesky", xmlns);

            XmlElement config = doc.CreateElement("config", xmlns);
            foreach (KeyValuePair<string, string> keyValuePair in _configuration)
            {
                XmlElement opt = doc.CreateElement("option", xmlns);
                opt.SetAttribute("name", keyValuePair.Key);
                opt.SetAttribute("value", keyValuePair.Value);
                config.AppendChild(opt);
            }
            root.AppendChild(config);

            XmlElement stalkselem = doc.CreateElement("stalks", xmlns);
            foreach (KeyValuePair<string, Stalk> kvp in stalks)
            {
                stalkselem.AppendChild(kvp.Value.ToXmlFragment(doc, xmlns));
            }
            root.AppendChild(stalkselem);

            XmlElement userelement = doc.CreateElement("users", xmlns);

            foreach (KeyValuePair<string,AccessListEntry> kvp in userlist)
            {
                AccessListEntry accessListEntry = kvp.Value;
                XmlElement u = doc.CreateElement("user", xmlns);
                u.SetAttribute("access", accessListEntry.AccessLevel.ToString());
                u.SetAttribute("hostmask", accessListEntry.HostnameMask);
                userelement.AppendChild(u);
       
            }
            root.AppendChild(userelement);
            
            XmlElement estalklog = doc.CreateElement("stalklog", xmlns);
            foreach (StalkLogItem stalkLogItem in stalklog)
            {
                estalklog.AppendChild(stalkLogItem.toXmlFragment(doc, xmlns));
            }

            root.AppendChild(estalklog);

            doc.AppendChild(root);

            doc.Save(configurationFileName);
        }
   
        public StalkLogItem[] RetrieveStalkLog()
        {
            StalkLogItem[] l = stalklog.ToArray();
            stalklog.Clear();
            return l;
        }
    }
}
