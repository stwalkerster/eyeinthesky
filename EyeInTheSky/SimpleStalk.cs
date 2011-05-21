using System;
using System.Text.RegularExpressions;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky
{
    public class SimpleStalk : Stalk
    {
        public SimpleStalk(string flag) : base(flag)
        {

        }

        private SimpleStalk(string flag, string time, string time2, string mailflag, string descr)
            : base(flag, time, time2, mailflag, descr)
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

        public StalkNode getEquivalentStalkTree()
        {
            // all three
            if((hasusercheck)&&(haspagecheck)&&(hassummarycheck))
            {
                AndNode a1 = new AndNode();
                AndNode a2 = new AndNode();
                LeafNode n1 = new UserStalkNode();
                LeafNode n2 = new PageStalkNode();
                LeafNode n3 = new SummaryStalkNode();
                n1.setMatchExpression(user.ToString());
                n2.setMatchExpression(page.ToString());
                n3.setMatchExpression(summary.ToString());
                a1.LeftChildNode = n2;
                a2.LeftChildNode = n1;
                a2.RightChildNode = n3;
                a1.RightChildNode = a2;
                return a1;
            }

            // missing one
            if ((hasusercheck) && (haspagecheck) && (!hassummarycheck))
            {
                AndNode a = new AndNode();
                LeafNode n1 = new UserStalkNode();
                LeafNode n2 = new PageStalkNode();
                n1.setMatchExpression(user.ToString());
                n2.setMatchExpression(page.ToString());
                a.LeftChildNode = n1;
                a.RightChildNode = n2;
                return a;
            }
            if ((hasusercheck) && (!haspagecheck) && (hassummarycheck))
            {
                AndNode a = new AndNode();
                LeafNode n1 = new UserStalkNode();
                LeafNode n2 = new SummaryStalkNode();
                n1.setMatchExpression(user.ToString());
                n2.setMatchExpression(summary.ToString());
                a.LeftChildNode = n1;
                a.RightChildNode = n2;
                return a;
            }
            if ((!hasusercheck) && (haspagecheck) && (hassummarycheck))
            {
                AndNode a = new AndNode();
                LeafNode n1 = new SummaryStalkNode();
                LeafNode n2 = new PageStalkNode();
                n1.setMatchExpression(summary.ToString());
                n2.setMatchExpression(page.ToString());
                a.LeftChildNode = n1;
                a.RightChildNode = n2;
                return a;
            }

            // missing two
            if ((hasusercheck) && (!haspagecheck) && (!hassummarycheck))
            {
                LeafNode n = new UserStalkNode();
                n.setMatchExpression(user.ToString());
                return n;
            }
            if ((!hasusercheck) && (haspagecheck) && (!hassummarycheck))
            {
                LeafNode n = new PageStalkNode();
                n.setMatchExpression(page.ToString());
                return n;
            }
            if ((!hasusercheck) && (!haspagecheck) && (hassummarycheck))
            {
                LeafNode n = new SummaryStalkNode();
                n.setMatchExpression(summary.ToString());
                return n;
            }

            // missing all
            if ((!hasusercheck) && (!haspagecheck) && (!hassummarycheck))
            {
                return new FalseNode();
            }

            throw new Exception();
        }

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

            if (this.mail)
                EyeInTheSkyBot.config.LogStalkTrigger(flag, rc);

            this.LastTriggerTime = DateTime.Now;
            return true;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("stalk", xmlns);
            e.SetAttribute("flag", flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString());
            e.SetAttribute("lasttrigger", LastTriggerTime.ToString());
            e.SetAttribute("mail", this.mail.ToString());
            e.SetAttribute("description", this.Description);

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
            string lastupdtime = time == null ? DateTime.Now.ToString() : time.Value;

            time = element.Attributes["lasttrigger"];
            string lasttrigtime = time == null ? DateTime.Parse("1/1/1970 00:00:00").ToString() : time.Value;

            string mailflag = element.GetAttribute("mail");
            string descr = element.GetAttribute("description");

            SimpleStalk s = new SimpleStalk(element.Attributes["flag"].Value, lastupdtime, lasttrigtime, mailflag, descr);
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