using System;
using System.Globalization;
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
            _baseNode = new FalseNode();
        }

        public ComplexStalk(string flag, string timeupd, string timetrig, string mailflag, string descr, string expiryTime, string immediatemail, string enabled) : base(flag)
        {
            
            if (flag == "")
                throw new ArgumentOutOfRangeException();
            this.flag = flag;

            if (!bool.TryParse(mailflag, out _mail))
                _mail = true; 
            
            if (!bool.TryParse(immediatemail, out _immediatemail))
                _immediatemail = false;

            if (!bool.TryParse(enabled, out _enabled))
                _enabled = true;


            _lastUpdateTime = DateTime.Parse(timeupd);

            _lastTriggerTime = DateTime.Parse(timetrig);

            _description = descr;

            this.expiryTime = DateTime.Parse(expiryTime);

            _baseNode = new FalseNode();
        }

        private StalkNode _baseNode;
        private DateTime _lastUpdateTime = DateTime.Now;
        private DateTime _lastTriggerTime = DateTime.Parse("1/1/1970 00:00:00");
        private bool _mail = true;
        private string _description = "";
        private DateTime _expiryTime = DateTime.MaxValue;
        private bool _immediatemail;
        private bool _enabled;


        public DateTime LastUpdateTime
        {
            get { return _lastUpdateTime; }
            protected set { _lastUpdateTime = value; }
        }

        public DateTime LastTriggerTime
        {
            get { return _lastTriggerTime; }
            protected set { _lastTriggerTime = value; }
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
            get { return _description; }
            set
            {
                _description = value;
                _lastUpdateTime = DateTime.Now;
            }
        }

        public DateTime expiryTime
        {
            get { return _expiryTime; }
            set
            {
                _expiryTime = value;
                _lastUpdateTime = DateTime.Now;
            }
        }

        public bool isActive()
        {
            if (DateTime.Now > expiryTime)
                return false;
            return enabled;
        }

        public override bool match(RecentChange rc)
        {
            if (!isActive())
                return false;

            bool success = _baseNode.match(rc);
            if(success)
            {
                LastTriggerTime = DateTime.Now;

                if (mail && bool.Parse(EyeInTheSkyBot.Config["logstalks"]))
                    EyeInTheSkyBot.Config.LogStalkTrigger(flag, rc);

                if (immediatemail)
                    immediateMail(rc);

                return true;
            }
            return false;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            e.SetAttribute("flag", flag);
            e.SetAttribute("lastupdate", LastUpdateTime.ToString(CultureInfo.InvariantCulture));
            e.SetAttribute("lasttrigger", LastTriggerTime.ToString(CultureInfo.InvariantCulture));
            e.SetAttribute("mail", mail.ToString());
            e.SetAttribute("immediatemail", immediatemail.ToString());
            e.SetAttribute("description", Description);
            e.SetAttribute("expiry", expiryTime.ToString(CultureInfo.InvariantCulture));
            e.SetAttribute("enabled", enabled.ToString());

            e.AppendChild(_baseNode.toXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "Flag: " + flag + ", Last modified: "+LastUpdateTime+", Type: Complex " + _baseNode;
        }

        public static new Stalk newFromXmlElement(XmlElement element)
        {
            XmlAttribute time = element.Attributes["lastupdate"];
            string lastupdtime = time == null ? DateTime.Now.ToString(CultureInfo.InvariantCulture) : time.Value;
            time = element.Attributes["lasttrigger"];
            string lastriggertime = time == null ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture) : time.Value;
            time = element.Attributes["expiry"];
            string exptime = time == null ? DateTime.MaxValue.ToString(CultureInfo.InvariantCulture) : time.Value;

            string mailflag = element.GetAttribute("mail");
            string immMail = element.GetAttribute("immediatemail");
            string enbld = element.GetAttribute("enabled");
            string descr = element.GetAttribute("description");

            var s = new ComplexStalk(element.Attributes["flag"].Value, lastupdtime, lastriggertime, mailflag, descr, exptime, immMail,enbld);
            
            if(element.HasChildNodes)
            {
                StalkNode n = StalkNode.newFromXmlFragment(element.FirstChild);
                s._baseNode = n;
            }

            return s;
        }

        public void setSearchTree(StalkNode node, bool isupdate)
        {
            if (isupdate)
                LastUpdateTime = DateTime.Now;

            _baseNode = node;
        }

        public StalkNode getSearchTree()
        {
            return _baseNode;
        }
    
        public void immediateMail(RecentChange rc)
        {
            string arg1 = Flag;
            string arg2 = rc.Page;
            string arg3 = rc.User;
            string arg4 = rc.EditSummary;
            string arg5 = rc.Url;
            string arg6 = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
            string arg7 = Description;
            string arg8 = _baseNode.ToString();
            string arg9 = rc.EditFlags;

            string template = new StreamReader("Templates/individual.txt").ReadToEnd();
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

            var mailMessage = new MailMessage
                                   {
                                       From = new MailAddress("eyeinthesky@eyeinthesky.im"),
                                       Subject = "[EyeInTheSky] '" + Flag + "' notification",
                                       Body = template
                                   };

            mailMessage.To.Add("simon@stwalkerster.co.uk");

            var client = new SmtpClient("mail.srv.stwalkerster.net");

            client.Send(mailMessage);
        }
    }
}