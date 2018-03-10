namespace EyeInTheSky.Model
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;

    public class ComplexStalk : IStalk
    {
        public ComplexStalk(string flag)
        {
            this.Flag = flag;
            this.baseNode = new FalseNode();
        }

        internal ComplexStalk(
            string flag,
            DateTime lastUpdateTime,
            DateTime lastTriggerTime,
            string description,
            DateTime expiryTime,
            bool mailEnabled,
            bool isEnabled,
            StalkNode baseNode)
        {
            this.Flag = flag;
            this.lastUpdateTime = lastUpdateTime;
            this.lastTriggerTime = lastTriggerTime;
            this.description = description;
            this.expiryTime = expiryTime;
            this.mailEnabled = mailEnabled;
            this.isEnabled = isEnabled;
            this.baseNode = baseNode;
        }

        private StalkNode baseNode;
        private DateTime lastUpdateTime = DateTime.Now;
        private DateTime lastTriggerTime = DateTime.MinValue;
        private string description = "";
        private DateTime expiryTime = DateTime.MaxValue;
        private bool mailEnabled = true;
        private bool isEnabled;

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
    }
}