namespace EyeInTheSky.Model.StalkNodes
{
    using System;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("expiry")]
    public class ExpiryNode : SingleChildLogicalNode
    {
        private DateTime expiry = DateTime.MaxValue;

        public DateTime Expiry
        {
            get { return this.expiry; }
            set { this.expiry = value; }
        }

        public void SetExpiry(string expiryValue)
        {
            this.Expiry = DateTime.Parse(expiryValue);
        }

        public string GetExpiryString()
        {
            return this.Expiry.ToString("c");
        }
        
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (DateTime.UtcNow > this.Expiry)
            {
                return false;
            }
            
            return this.ChildNode.Match(rc, forceMatch);
        }
        
        public override string ToString()
        {
            return "(exp:" + this.ChildNode +  ")";
        }
    }
}