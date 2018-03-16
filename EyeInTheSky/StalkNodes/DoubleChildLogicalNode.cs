namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public abstract class DoubleChildLogicalNode : LogicalNode
    {
        public IStalkNode LeftChildNode { get; set; }
        public IStalkNode RightChildNode { get; set; }
        
        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);
            
            if (this.LeftChildNode == null)
            {
                throw new InvalidOperationException("No left child node defined!");
            }
            
            if (this.RightChildNode == null)
            {
                throw new InvalidOperationException("No right child node defined!");
            }
        }
    }
}