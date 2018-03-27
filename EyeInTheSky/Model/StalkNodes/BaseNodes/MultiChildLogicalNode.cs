namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model.Interfaces;

    public abstract class MultiChildLogicalNode : LogicalNode
    {
        protected MultiChildLogicalNode()
        {
            this.ChildNodes = new List<IStalkNode>();
        }
        
        public List<IStalkNode> ChildNodes { get; set; }

        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);

            if (this.ChildNodes == null)
            {
                throw new InvalidOperationException("Child node list is null");
                
            }
            
            if (!this.ChildNodes.Any())
            {
                throw new InvalidOperationException("No child nodes present");
            }
        }
    }
}