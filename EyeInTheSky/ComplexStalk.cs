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

        public ComplexStalk(string flag, string timeupd, string timetrig, string mailflag, string descr, string expiryTime) : base(flag)
        {
            
            if (flag == "")
                throw new ArgumentOutOfRangeException();
            this.flag = flag;

            if (!bool.TryParse(mailflag, out this._mail))
                this._mail = true;

            this.lastUpdateTime = DateTime.Parse(timeupd);

            this.lastTriggerTime = DateTime.Parse(timetrig);

            this.description = descr;

            this.expiryTime = DateTime.Parse(expiryTime);

            baseNode = new FalseNode();
        }

        private StalkNode baseNode;
        private DateTime lastUpdateTime = DateTime.Now;
        private DateTime lastTriggerTime = DateTime.Parse("1/1/1970 00:00:00");
        private bool _mail = true;
        private string description = "";
        private DateTime _expiryTime = DateTime.MaxValue;


        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime; }
            protected set { lastUpdateTime = value; }
        }

        public DateTime LastTriggerTime
        {
            get { return lastTriggerTime; }
            protected set { lastTriggerTime = value; }
        }



        public bool mail
        {
            get { return _mail; }
            set { _mail = value; }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                lastUpdateTime = DateTime.Now;
            }
        }

        public DateTime expiryTime
        {
            get { return _expiryTime; }
            set
            {
                _expiryTime = value;
                lastUpdateTime = DateTime.Now;
            }
        }

        public bool isActive()
        {
            if (DateTime.Now > this.expiryTime)
                return false;
            return true;
        }

        public override bool match(RecentChange rc)
        {
            if (!isActive())
                return false;

            bool success = baseNode.match(rc);
            if(success)
            {
                this.LastTriggerTime = DateTime.Now;

                if (this.mail && bool.Parse(EyeInTheSkyBot.config["logstalks"]))
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
            string lastriggertime = time == null ? DateTime.MinValue.ToString() : time.Value;
            time = element.Attributes["expiry"];
            string exptime = time == null ? DateTime.MaxValue.ToString() : time.Value;

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