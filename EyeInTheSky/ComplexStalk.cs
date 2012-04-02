using System;
using System.IO;
using System.Net.Mail;
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

        public ComplexStalk(string flag, string timeupd, string timetrig, string mailflag, string descr, string expiryTime, string immediatemail, string enabled) : base(flag)
        {
            
            if (flag == "")
                throw new ArgumentOutOfRangeException();
            this.flag = flag;

            if (!bool.TryParse(mailflag, out this._mail))
                this._mail = true; 
            
            if (!bool.TryParse(immediatemail, out this._immediatemail))
                this._immediatemail = false;

            if (!bool.TryParse(enabled, out this._enabled))
                this._enabled = true;


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
        private bool _immediatemail;
        private bool _enabled;


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

        public bool enabled { get { return _enabled; } set { _enabled = value; } }

        public bool immediatemail { get { return _immediatemail; } set { _immediatemail = value; } }

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
            return this.enabled;
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

                if (this.immediatemail)
                    this.immediateMail(rc);

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
            e.SetAttribute("immediatemail", this.immediatemail.ToString());
            e.SetAttribute("description", this.Description);
            e.SetAttribute("expiry", this.expiryTime.ToString());
            e.SetAttribute("enabled", this.enabled.ToString());

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
            string immMail = element.GetAttribute("immediatemail");
            string enbld = element.GetAttribute("enabled");
            string descr = element.GetAttribute("description");

            ComplexStalk s = new ComplexStalk(element.Attributes["flag"].Value, lastupdtime, lastriggertime, mailflag, descr, exptime, immMail,enbld);
            
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
    
        public void immediateMail(RecentChange rc)
        {
            string arg1 = this.Flag;
            string arg2 = rc.Page;
            string arg3 = rc.User;
            string arg4 = rc.EditSummary;
            string arg5 = rc.Url;
            string arg6 = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            string arg7 = this.Description;
            string arg8 = this.ToString();
            string arg9 = rc.EditFlags;

            string template = new StreamReader("individual.txt").ReadToEnd();
            template = template
                .Replace("$1", arg1)
                .Replace("$2", arg2)
                .Replace("$3", arg3)
                .Replace("$4", arg4)
                .Replace("$5", arg5)
                .Replace("$6", arg6)
                .Replace("$7", arg7)
                .Replace("$8", arg8)
                .Replace("$9", arg9)
                ;

            MailMessage mailMessage = new MailMessage
                                   {
                                       From = new MailAddress("eyeinthesky@helpmebot.org.uk"),
                                       Subject = "EyeInTheSky notification",
                                       Body = template;
                                   };

            mailMessage.To.Add("stwalkerster@helpmebot.org.uk");

            SmtpClient client = new SmtpClient("mail.helpmebot.org.uk");

            client.Send(mailMessage);
        }
    }
}