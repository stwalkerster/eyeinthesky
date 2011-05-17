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
        private DateTime ts;

        public StalkLogItem(string flag, RecentChange rcitem)
        {
            stalk = flag;
            rc = rcitem;
            ts = DateTime.Now;
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
            esli.SetAttribute("stalkflag", this.stalk);
            esli.SetAttribute("timestamp", DateTime.Now.ToString());

            XmlElement u = doc.CreateElement("user", xmlns);
            u.SetAttribute("value", rc.User);
            XmlElement p = doc.CreateElement("page", xmlns);
            p.SetAttribute("value", rc.Page);
            XmlElement url = doc.CreateElement("url", xmlns);
            url.SetAttribute("value",rc.Url );
            XmlElement s = doc.CreateElement("summary", xmlns);
            s.SetAttribute("value", rc.EditSummary);
            XmlElement f = doc.CreateElement("flags", xmlns);
            f.SetAttribute("value", rc.EditFlags );
            XmlElement sd = doc.CreateElement("sizediff", xmlns);
            sd.SetAttribute("value", rc.SizeDifference.ToString());

            esli.AppendChild(u);
            esli.AppendChild(p);
            esli.AppendChild(url);
            esli.AppendChild(s);
            esli.AppendChild(f);
            esli.AppendChild(sd);

            return esli;
        }

        public override string ToString()
        {
            return string.Format(
                "[{0}] Stalked edit {1} to page \"{2}\" at {5} by [[User:{3}]], summary: {4}",
                stalk, rc.Url,
                rc.Page,
                rc.User,
                rc.EditSummary, ts.ToString()
                );
        }

        internal static StalkLogItem newFromXmlElement(XmlElement element)
        {
            throw new NotImplementedException();
        }
    }
}
