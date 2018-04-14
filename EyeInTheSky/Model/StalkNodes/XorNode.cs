namespace EyeInTheSky.Model.StalkNodes
{
    using System;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("xor")]
    public class XorNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            var leftResult = this.LeftChildNode.Match(rc, false);
            var rightResult = this.RightChildNode.Match(rc, false);
            
            if(leftResult.HasValue && rightResult.HasValue && leftResult.Value == rightResult.Value)
            {
                return false;
            }

            if(leftResult.HasValue && rightResult.HasValue)
            {
                // leftResult!=rightResult is always true because above condition, hence excluded
                
                return true;
            }

            if(!forceMatch)
            {
                return null;
            }
            
            leftResult = leftResult ?? this.LeftChildNode.Match(rc, true);
            rightResult = rightResult ?? this.RightChildNode.Match(rc, true);

            if (!leftResult.HasValue || !rightResult.HasValue)
            {
                return null;
            }

            return leftResult.Value != rightResult.Value;
        }

        public override string ToString()
        {
            return "(^:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}