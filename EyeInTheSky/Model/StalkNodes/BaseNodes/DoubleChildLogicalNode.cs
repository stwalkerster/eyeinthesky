namespace EyeInTheSky.Model.StalkNodes.BaseNodes
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

        protected override void PopulateClone(IStalkNode node)
        {
            var dcln = (DoubleChildLogicalNode) node;
            dcln.LeftChildNode = (IStalkNode) this.LeftChildNode.Clone();
            dcln.RightChildNode = (IStalkNode) this.RightChildNode.Clone();
        }
    }
}