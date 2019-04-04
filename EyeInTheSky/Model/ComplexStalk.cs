namespace EyeInTheSky.Model
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    public class ComplexStalk : IStalk
    {
        public ComplexStalk(string flag)
        {
            this.LastTriggerTime = DateTime.MinValue;
            this.LastUpdateTime = DateTime.Now;
            this.Identifier = flag;
            this.baseNode = new FalseNode();
            this.Subscribers = new List<StalkUser>();
        }

        internal ComplexStalk(
            string flag,
            DateTime? lastUpdateTime,
            DateTime? lastTriggerTime,
            string description,
            DateTime? expiryTime,
            bool isEnabled,
            int triggerCount,
            string lastMessageId,
            string watchChannel,
            TimeSpan? dynamicExpiry)
        {
            this.Identifier = flag;
            this.LastUpdateTime = lastUpdateTime;
            this.LastTriggerTime = lastTriggerTime;
            this.TriggerCount = triggerCount;
            this.LastMessageId = lastMessageId;
            this.WatchChannel = watchChannel;
            this.dynamicExpiry = dynamicExpiry;
            this.description = description;
            this.expiryTime = expiryTime;
            this.isEnabled = isEnabled;
            this.Subscribers = new List<StalkUser>();
        }

        private IStalkNode baseNode;
        private string description;
        private DateTime? expiryTime;
        private bool isEnabled;
        private TimeSpan? dynamicExpiry;

        public List<StalkUser> Subscribers { get; private set; }

        public string Identifier { get; private set; }

        public DateTime? LastUpdateTime { get; private set; }

        public DateTime? LastTriggerTime { get; set; }

        public int TriggerCount { get; set; }

        public string LastMessageId { get; set; }

        public string Channel { get; set; }

        public string WatchChannel { get; set; }

        public TimeSpan? DynamicExpiry
        {
            get { return this.dynamicExpiry; }
            set
            {
                this.dynamicExpiry = value;
                this.LastUpdateTime = DateTime.Now;
            }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.LastUpdateTime = DateTime.Now;
            }
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

        public bool TriggerDynamicExpiry()
        {
            if (this.dynamicExpiry.HasValue)
            {
                var proposedExpiry = DateTime.Now + this.dynamicExpiry.Value;

                if (this.expiryTime < proposedExpiry)
                {
                    this.expiryTime = proposedExpiry;
                    return true;
                }
            }

            return false;
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

        public bool IsExpiringSoon()
        {
            if (!this.ExpiryTime.HasValue)
            {
                return false;
            }

            if (DateTime.Now > this.ExpiryTime)
            {
                return false;
            }

            var remaining = this.ExpiryTime.Value - DateTime.Now;
            if (remaining.TotalDays < 7)
            {
                return true;
            }

            return false;
        }

        public bool Match(IRecentChange rc)
        {
            if (!this.IsActive())
            {
                return false;
            }

            return this.baseNode.Match(rc);
        }

        internal void SetStalkTree(IStalkNode tree)
        {
            this.baseNode = tree;
        }
    }
}