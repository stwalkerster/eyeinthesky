namespace EyeInTheSky.Model.StalkNodes.NumericNodes
{
    using System;

    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("infixnumeric")]
    public class InfixNumericLogicalNode : LogicalNode
    {
        public INumberProviderNode LeftChildNode { get; set; }
        public INumberProviderNode RightChildNode { get; set; }

        public string Operator
        {
            get;
            set;
        }
        
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (this.LeftChildNode.GetValue(rc, forceMatch) == null ||
                this.RightChildNode.GetValue(rc, forceMatch) == null)
            {
                return null;
            }
            
            switch (this.Operator)
            {
                case "==":
                    return this.LeftChildNode.GetValue(rc, forceMatch) == this.RightChildNode.GetValue(rc, forceMatch);
                case "<>":
                case "!=":
                    return this.LeftChildNode.GetValue(rc, forceMatch) != this.RightChildNode.GetValue(rc, forceMatch);
                case ">":
                    return this.LeftChildNode.GetValue(rc, forceMatch) > this.RightChildNode.GetValue(rc, forceMatch);
                case "<":
                    return this.LeftChildNode.GetValue(rc, forceMatch) < this.RightChildNode.GetValue(rc, forceMatch);
                case ">=":
                    return this.LeftChildNode.GetValue(rc, forceMatch) >= this.RightChildNode.GetValue(rc, forceMatch);
                case "<=":
                    return this.LeftChildNode.GetValue(rc, forceMatch) <= this.RightChildNode.GetValue(rc, forceMatch);
            }

            return null;
        }

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
            
            if (this.Operator == null)
            {
                throw new InvalidOperationException("No operator defined!");
            }
        }

        protected override void PopulateClone(IStalkNode node)
        {
            var dcln = (InfixNumericLogicalNode) node;
            dcln.LeftChildNode = (INumberProviderNode) this.LeftChildNode.Clone();
            dcln.RightChildNode = (INumberProviderNode) this.RightChildNode.Clone();
            dcln.Operator = this.Operator;
        }

        public override string ToString()
        {
            return string.Format("(ifnlogic: {0} {1} {2})", this.LeftChildNode, this.Operator, this.RightChildNode);
        }
    }
}