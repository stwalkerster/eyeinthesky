namespace EyeInTheSky.Model
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;
    using Microsoft.Practices.ServiceLocation;

    public class ComplexStalk : IStalk
    {
        public ComplexStalk(string flag)
        {
            this.Flag = flag;
            this.baseNode = new FalseNode();
        }

        public ComplexStalk(string flag,
            string timeupd,
            string timetrig,
            string descr,
            string expiryTime,
            string immediatemail,
            string enabled)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger>()
                .CreateChildLogger("ComplexStalk")
                .CreateChildLogger(flag);

            if (flag == "")
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Flag = flag;

            if (!bool.TryParse(immediatemail, out this.mailEnabled))
            {
                this.mailEnabled = false;
            }

            if (!bool.TryParse(enabled, out this.isEnabled))
            {
                this.isEnabled = true;
            }

            this.ParseDate(flag, timeupd, logger, out this.lastUpdateTime, "last update time");
            this.ParseDate(flag, timetrig, logger, out this.lastTriggerTime, "last trigger time");

            this.description = descr;

            this.ParseDate(flag, expiryTime, logger, out this.expiryTime, "expiry time");

            this.baseNode = new FalseNode();
        }

        private void ParseDate(string flagName, string input, ILogger logger, out DateTime result, string propName)
        {
            if (!DateTime.TryParseExact(
                input,
                "O",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out result))
            {
                logger.WarnFormat("Unknown date format in stalk '{0}' {2}: {1}", flagName, input, propName);

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    var err = string.Format("Failed date parse for stalk '{0}' {2}: {1}", flagName, input, propName);

                    logger.Error(err);
                    throw new FormatException(err);
                }
            }
        }

        private StalkNode baseNode;
        private DateTime lastUpdateTime = DateTime.Now;
        private DateTime lastTriggerTime = DateTime.MinValue;
        private string description = "";
        private DateTime expiryTime = DateTime.MaxValue;
        private bool mailEnabled = true;
        private bool isEnabled;

        /// <inheritdoc />
        public string Flag { get; private set; }

        public DateTime LastUpdateTime
        {
            get { return this.lastUpdateTime; }
            private set { this.lastUpdateTime = value; }
        }

        public DateTime LastTriggerTime
        {
            get { return this.lastTriggerTime; }
            set { this.lastTriggerTime = value; }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.isEnabled = value; }
        }

        public bool MailEnabled
        {
            get { return this.mailEnabled; }
            set { this.mailEnabled = value; }
        }

        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                this.lastUpdateTime = DateTime.Now;
            }
        }

        public DateTime ExpiryTime
        {
            get { return this.expiryTime; }
            set
            {
                this.expiryTime = value;
                this.lastUpdateTime = DateTime.Now;
            }
        }

        public StalkNode SearchTree
        {
            get { return this.baseNode; }

            set
            {
                this.LastUpdateTime = DateTime.Now;
                this.baseNode = value;
            }
        }

        public bool IsActive()
        {
            if (DateTime.Now > this.ExpiryTime)
            {
                return false;
            }

            return this.IsEnabled;
        }

        public bool Match(IRecentChange rc)
        {
            if (!this.IsActive())
            {
                return false;
            }

            return this.baseNode.Match(rc);
        }

        public XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            XmlElement e = doc.CreateElement("complexstalk", xmlns);
            e.SetAttribute("flag", this.Flag);
            e.SetAttribute("lastupdate", this.LastUpdateTime.ToString("O"));
            e.SetAttribute("lasttrigger", this.LastTriggerTime.ToString("O"));
            e.SetAttribute("immediatemail", this.MailEnabled.ToString());
            e.SetAttribute("description", this.Description);
            e.SetAttribute("enabled", this.IsEnabled.ToString());
            e.SetAttribute("expiry", this.ExpiryTime.ToString("O"));

            e.AppendChild(this.baseNode.ToXmlFragment(doc, xmlns));
            return e;
        }

        public override string ToString()
        {
            return "Flag: " + this.Flag + ", Last modified: " + this.LastUpdateTime + ", Type: Complex "
                   + this.baseNode;
        }

        public static IStalk NewFromXmlElement(XmlElement element)
        {
            XmlAttribute time = element.Attributes["lastupdate"];
            string lastupdtime = time == null ? DateTime.Now.ToString(CultureInfo.InvariantCulture) : time.Value;
            time = element.Attributes["lasttrigger"];
            string lastriggertime =
                time == null ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture) : time.Value;
            time = element.Attributes["expiry"];
            string exptime = time == null ? DateTime.MaxValue.ToString(CultureInfo.InvariantCulture) : time.Value;

            string immMail = element.GetAttribute("immediatemail");
            string enbld = element.GetAttribute("enabled");
            string descr = element.GetAttribute("description");

            var s = new ComplexStalk(
                element.Attributes["flag"].Value,
                lastupdtime,
                lastriggertime,
                descr,
                exptime,
                immMail,
                enbld);

            if (element.HasChildNodes)
            {
                StalkNode n = StalkNode.NewFromXmlFragment(element.FirstChild);
                s.baseNode = n;
            }

            return s;
        }
    }
}