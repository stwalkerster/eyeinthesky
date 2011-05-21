using System;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky
{
    public class ComplexStalk : Stalk
    {
        public ComplexStalk(string flag)
            : base(flag)
        {
            baseNode = new FalseNode();
        }

        public ComplexStalk(string flag, string timeupd, string timetrig, string mailflag, string descr, string expiryTime)
            : base(flag, timeupd, timetrig, mailflag, descr, expiryTime)
        {
            baseNode = new FalseNode();

        }

        private StalkNode baseNode;


        public override bool match(RecentChange rc)
        {
            bool success = baseNode.match(rc);
            if(success)
            {
                this.LastTriggerTime = DateTime.Now;

                if (this.mail)
                    EyeInTheSkyBot.config.LogStalkTrigger(flag, rc);

                return true;
            }
            return false;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            e.SetAttribute("flag", this.flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString());
            e.SetAttribute("lasttrigger", LastTriggerTime.ToString());
            e.SetAttribute("mail", this.mail.ToString());
            e.SetAttribute("description", this.Description);
            e.SetAttribute("expiry", this.expiryTime.ToString());

            e.AppendChild(baseNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "Flag: " + flag + ", Last modified: "+this.LastUpdateTime+", Type: Complex " + baseNode.ToString();
        }

        public static new Stalk newFromXmlElement(XmlElement element)
        {
            XmlAttribute time = element.Attributes["lastupdate"];
            string lastupdtime = time == null ? DateTime.Now.ToString() : time.Value;
            time = element.Attributes["lasttrigger"];
            string lastriggertime = time == null ? DateTime.Parse("1/1/1970 00:00:00").ToString() : time.Value;
            time = element.Attributes["expiry"];
            string exptime = time == null ? DateTime.Parse("1/1/1970 00:00:00").ToString() : time.Value;

            string mailflag = element.GetAttribute("mail");
            string descr = element.GetAttribute("description");

            ComplexStalk s = new ComplexStalk(element.Attributes["flag"].Value, lastupdtime, lastriggertime, mailflag,descr,exptime);
            
            if(element.HasChildNodes)
            {
                StalkNode n = StalkNode.newFromXmlFragment(element.FirstChild);
                s.baseNode = n;
            }

            return s;
        }

        public void setSearchTree(StalkNode node, bool isupdate)
        {
            if (isupdate)
                this.LastUpdateTime = DateTime.Now;

            baseNode = node;
        }
    }
}