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
    class Configuration
    {
        private readonly string configurationFileName;
        private Dictionary<string, string> _configuration;


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
                return string.Empty;
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
            }
        }


        public void rehash()
        {
            initialise();
        }

        private void initialise()
        {
            if (!new FileInfo(configurationFileName).Exists)
                throw new FileNotFoundException();

            XPathDocument xPathDocument = new XPathDocument(configurationFileName);
            XPathNavigator navigator = xPathDocument.CreateNavigator();

            XPathNodeIterator xpni = navigator.Select("//option");
            _configuration = new Dictionary<string, string>();
            while (xpni.MoveNext())
            {
                _configuration.Add(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("value", ""));
            }
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
