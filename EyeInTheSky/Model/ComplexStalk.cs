namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;

    public class ComplexStalk : IStalk
    {
        public ComplexStalk(string flag)
        {
            this.LastTriggerTime = DateTime.MinValue;
            this.LastUpdateTime = DateTime.Now;
            this.Flag = flag;
            this.baseNode = new FalseNode();
        }

        internal ComplexStalk(
            string flag,
            DateTime? lastUpdateTime,
            DateTime? lastTriggerTime,
            string description,
            DateTime? expiryTime,
            bool mailEnabled,
            bool isEnabled,
            int triggerCount,
            IStalkNode baseNode)
        {
            this.Flag = flag;
            this.LastUpdateTime = lastUpdateTime;
            this.LastTriggerTime = lastTriggerTime;
            this.TriggerCount = triggerCount;
            this.description = description;
            this.expiryTime = expiryTime;
            this.mailEnabled = mailEnabled;
            this.isEnabled = isEnabled;
            this.baseNode = baseNode;
        }

        private IStalkNode baseNode;
        private string description;
        private DateTime? expiryTime;
        private bool mailEnabled = true;
        private bool isEnabled;

        public string Flag { get; private set; }

        public DateTime? LastUpdateTime { get; private set; }

        public DateTime? LastTriggerTime { get; set; }
        
        public int TriggerCount { get; set; }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
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

        public DateTime? ExpiryTime
        {
            get { return this.expiryTime; }
            set
            {
                this.expiryTime = value;
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

        public override string ToString()
        {
            return "Flag: " + this.Flag + ", Last modified: " + this.LastUpdateTime + ", Type: Complex "
                   + this.baseNode;
        }
    }
}