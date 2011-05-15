using System.Text.RegularExpressions;
using System.Xml;

namespace EyeInTheSky
{
    public class SimpleStalk : Stalk
    {
        public SimpleStalk(string flag) : base(flag)
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

        public void setUserSearch(string regex)
        {
            hasusercheck = true;
            user = new Regex(regex);
        }
        public void setPageSearch(string regex)
        {
            haspagecheck = true;
            page = new Regex(regex);
        }
        public void setSummarySearch(string regex)
        {
            hassummarycheck = true;
            summary = new Regex(regex);
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

            return true;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("stalk", xmlns);
            e.SetAttribute("flag", flag);

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
            SimpleStalk s = new SimpleStalk(element.Attributes["flag"].Value);
            foreach (XmlNode childNode in element.ChildNodes)
            {
                if(! (childNode is XmlElement))
                    continue;

                switch (childNode.Name)
                {
                    case "user":
                        s.setUserSearch(childNode.Attributes["value"].Value);
                        break;
                    case "page":
                        s.setPageSearch(childNode.Attributes["value"].Value);
                        break;
                    case "summary":
                        s.setSummarySearch(childNode.Attributes["value"].Value);
                        break;
                }
            }

            return s;
        }
    }
}