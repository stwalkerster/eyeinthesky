using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace EyeInTheSky
{
    public class SimpleStalk : Stalk
    {
        public SimpleStalk(string flag) : base(flag)
        {

        }

        private SimpleStalk(string flag, string time, string time2)
            : base(flag, time, time2)
        {
        }


        public bool HasUserSearch
        {
            get { return hasusercheck; }
        }

        public bool HasPageSearch
        {
            get { return haspagecheck; }
        }

        public bool HasSummarySearch
        {
            get { return hassummarycheck; }
        }

        private bool hasusercheck;
        private bool haspagecheck;
        private bool hassummarycheck;

        private Regex user, page, summary;


        public void setUserSearch(string regex, bool isupdate)
        {
            hasusercheck = true;
            user = new Regex(regex);

            if (isupdate)
                this.LastUpdateTime = DateTime.Now;
        }
        public void setPageSearch(string regex, bool isupdate)
        {
            haspagecheck = true;
            page = new Regex(regex);

            if (isupdate)
                this.LastUpdateTime = DateTime.Now;
        }
        public void setSummarySearch(string regex, bool isupdate)
        {
            hassummarycheck = true;
            summary = new Regex(regex);

            if (isupdate)
                this.LastUpdateTime = DateTime.Now;
        }

        public override bool match(RecentChange rc)
        {
            if (!(HasUserSearch || HasPageSearch || hassummarycheck))
                return false;

            if(HasUserSearch && ! user.Match(rc.User).Success)
            {
                return false;
            }

            if(HasPageSearch && ! page.Match(rc.Page).Success)
            {
                return false;
            }

            if (hassummarycheck && !summary.Match(rc.EditSummary).Success)
            {
                return false;
            }

            this.LastTriggerTime = DateTime.Now;
            return true;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("stalk", xmlns);
            e.SetAttribute("flag", flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString());
            e.SetAttribute("lasttrigger", LastTriggerTime.ToString());

            if (HasUserSearch)
            {
                XmlElement sub = doc.CreateElement("user", xmlns);
                sub.SetAttribute("value", user.ToString());
                e.AppendChild(sub);
            }

            if (HasPageSearch)
            {
                XmlElement sub = doc.CreateElement("page", xmlns);
                sub.SetAttribute("value", page.ToString());
                e.AppendChild(sub);
            }

            if (hassummarycheck)
            {
                XmlElement sub = doc.CreateElement("summary", xmlns);
                sub.SetAttribute("value", summary.ToString());
                e.AppendChild(sub);
            }

            return e;
        }

        public static new Stalk newFromXmlElement(XmlElement element)
        {
            XmlAttribute time = element.Attributes["lastupdate"];
            string timeval = time == null ? DateTime.Now.ToString() : time.Value;

            time = element.Attributes["lasttrigger"];
            string timeval2 = time == null ? DateTime.Parse("1/1/1970 00:00:00").ToString() : time.Value;

            SimpleStalk s = new SimpleStalk(element.Attributes["flag"].Value, timeval, timeval2);
            foreach (XmlNode childNode in element.ChildNodes)
            {
                if(! (childNode is XmlElement))
                    continue;

                switch (childNode.Name)
                {
                    case "user":
                        s.setUserSearch(childNode.Attributes["value"].Value, false);
                        break;
                    case "page":
                        s.setPageSearch(childNode.Attributes["value"].Value, false);
                        break;
                    case "summary":
                        s.setSummarySearch(childNode.Attributes["value"].Value, false);
                        break;
                }
            }

            return s;
        }
    }
}