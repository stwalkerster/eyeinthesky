namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    
    public class Template : ITemplate
    {
        private bool templateIsEnabled;
        private bool stalkIsEnabled;
        private bool mailEnabled;
        private string description;
        private IStalkNode baseNode;

        public Template(string flag)
        {
            this.Flag = flag;
            this.LastUpdateTime = DateTime.Now;
            this.baseNode = new FalseNode();
        }

        public Template(
            string flag,
            bool templateIsEnabled,
            bool stalkIsEnabled,
            bool mailEnabled,
            string description,
            DateTime? lastUpdateTime,
            TimeSpan? expiryDuration,
            IStalkNode baseNode)
        {
            this.templateIsEnabled = templateIsEnabled;
            this.stalkIsEnabled = stalkIsEnabled;
            this.mailEnabled = mailEnabled;
            this.description = description;
            this.baseNode = baseNode;
            this.Flag = flag;
            this.LastUpdateTime = lastUpdateTime;
            this.ExpiryDuration = expiryDuration;
        }

        public string Flag { get; private set; }
        public DateTime? LastUpdateTime { get; private set; }
        public TimeSpan? ExpiryDuration { get; set; }
        
        public bool TemplateIsEnabled
        {
            get { return this.templateIsEnabled; }
            set
            {
                this.templateIsEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }

        public bool StalkIsEnabled
        {
            get { return this.stalkIsEnabled; }
            set
            {
                this.stalkIsEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }
        
        public bool MailEnabled
        {
            get { return this.mailEnabled; }
            set { this.mailEnabled = value; 
                this.LastUpdateTime = DateTime.Now;}
        }
        
        public string Description
        {
            get { return this.description; }
            set
            {
                this.description = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }
        
        public IStalkNode SearchTree
        {
            get { return this.baseNode; }

            set
            {
                this.LastUpdateTime = DateTime.Now;
                this.baseNode = value;
            }
        }
    }
}