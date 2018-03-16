namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public abstract class SingleChildLogicalNode : LogicalNode
    {
        public IStalkNode ChildNode { get; set; }

        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);
            
            if (this.ChildNode == null)
            {
                throw new InvalidOperationException("No child node defined!");
            }
        }
    }
}
