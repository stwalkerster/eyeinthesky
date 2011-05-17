using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky
{
    class StalkLogItem
    {
        private string stalk;
        private RecentChange rc;

        public StalkLogItem(string flag, RecentChange rcitem)
        {
            stalk = flag;
            rc = rcitem;
        }

        public string Stalk
        {
            get { return stalk; }
        }

        public RecentChange RecentChangeItem
        {
            get { return rc; }
        }
    
        public XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement esli = doc.CreateElement("log", xmlns);
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        internal static StalkLogItem newFromXmlElement(XmlElement element)
        {
            throw new NotImplementedException();
        }
    }
}
