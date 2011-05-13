using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace EyeInTheSky
{
    public class Configuration
    {
        private readonly string configurationFileName;
        private Dictionary<string, string> _configuration;
        private StalkList userstalk;
        private StalkList pagestalk;
        private StalkList summarystalk;

        public Configuration(string fileName)
        {
            configurationFileName = fileName;

            initialise();
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

        public StalkList StalksUser
        {
            get { return userstalk; }
        }

        public StalkList StalksPage
        {
            get { return pagestalk; }
        }

        public StalkList StalksSummary
        {
            get { return summarystalk; }
        }

        public void delete(string configOption)
        {
            _configuration.Remove(configOption);
        }

        public void rehash()
        {
            initialise();
        }

        private void initialise()
        {
            if (!new FileInfo(configurationFileName).Exists)
            {
                File.Copy("DefaultConfiguration.xml", configurationFileName);
            }

            StreamReader sr = new StreamReader(configurationFileName);

            XPathDocument xPathDocument = new XPathDocument(sr);
            XPathNavigator navigator = xPathDocument.CreateNavigator();

            XmlNameTable xnt = navigator.NameTable;
            XmlNamespaceManager xnm = new XmlNamespaceManager(xnt);
            xnm.AddNamespace("isky", "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd");

            XPathNodeIterator xpni = navigator.Select("//isky:option", xnm);

            _configuration = new Dictionary<string, string>();
            while (xpni.MoveNext())
            {
                _configuration.Add(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("value", ""));
            }

            userstalk = StalkList.fetch(navigator.Select("//isky:stalks/isky:user/isky:stalk"));
            pagestalk = StalkList.fetch(navigator.Select("//isky:stalks/isky:page/isky:stalk"));
            summarystalk = StalkList.fetch(navigator.Select("//isky:stalks/isky:summary/isky:stalk"));
        }

        public void save()
        {
// ReSharper disable UseObjectOrCollectionInitializer
            XmlTextWriter xtw = new XmlTextWriter(configurationFileName, null);
// ReSharper restore UseObjectOrCollectionInitializer
            xtw.Formatting = Formatting.Indented;
            xtw.WriteStartDocument();
            {
                xtw.WriteStartElement("eyeinthesky",
                                      "https://github.com/stwalkerster/eyeinthesky/raw/master/EyeInTheSky/DataFileSchema.xsd");
                {
                    xtw.WriteStartElement("config");
                    foreach (KeyValuePair<string, string> keyValuePair in _configuration)
                    {
                        xtw.WriteStartElement("option");
                        xtw.WriteAttributeString("name", keyValuePair.Key);
                        xtw.WriteAttributeString("value", keyValuePair.Value);
                        xtw.WriteEndElement();
                    }
                    xtw.WriteEndElement();

                    xtw.WriteStartElement("stalks");
                    {
                        xtw.WriteStartElement("user");
                        {
                            
                        }
                        xtw.WriteEndElement();
                        xtw.WriteStartElement("page");
                        {

                        }
                        xtw.WriteEndElement();
                        xtw.WriteStartElement("summary");
                        {

                        }
                        xtw.WriteEndElement();
                    }
                    xtw.WriteEndElement();
                }
                xtw.WriteEndElement();
            }
            xtw.WriteEndDocument();

            xtw.Flush();
            xtw.Close();
        }
    }
}
