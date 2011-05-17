using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace EyeInTheSky
{
    public class StalkLogItem
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

        private StalkLogItem()
        {
        }

        public string Stalk
        {
            get { return stalk; }
        }

        public RecentChange RecentChangeItem
        {
            get { return rc; }
        }

        public DateTime Timestamp
        {
            get { return ts; }
        }

        public XmlElement toXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement esli = doc.CreateElement("log", xmlns);
            esli.SetAttribute("stalkflag", this.stalk);
            esli.SetAttribute("timestamp", Timestamp.ToString());

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
                rc.EditSummary, Timestamp.ToString()
                );
        }

        internal static StalkLogItem newFromXmlElement(XmlElement element)
        {
            StalkLogItem sli = new StalkLogItem
                                   {
                                       stalk = element.GetAttribute("stalkflag"),
                                       ts = DateTime.Parse(element.GetAttribute("timestamp"))
                                   };


            string user, page, url, summary, flags, sizediff;
            user = page = url = summary = flags = "";
            sizediff = "0";
            foreach (XmlNode childNode in element.ChildNodes)
            {
                XmlElement ce = (XmlElement) childNode;
                if (ce.Name == "user")
                    user = ce.GetAttribute("value");
                if (ce.Name == "page")
                    page = ce.GetAttribute("value");
                if (ce.Name == "url")
                    url = ce.GetAttribute("value");
                if (ce.Name == "summary")
                    summary = ce.GetAttribute("value");
                if (ce.Name == "flags")
                    flags = ce.GetAttribute("value");
                if (ce.Name == "sizediff")
                    sizediff = ce.GetAttribute("value");
 
            }

            RecentChange rc = new RecentChange(page, user, url, summary, flags, int.Parse(sizediff));
            sli.rc = rc;
            return sli;
        }
    }
}
