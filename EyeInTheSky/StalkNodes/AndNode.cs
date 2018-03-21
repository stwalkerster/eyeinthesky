using System;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("and")]
    public class AndNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            var leftResult = this.LeftChildNode.Match(rc, false);

            if(leftResult == false) {
                return false;
            }

            var rightResult = this.RightChildNode.Match(rc, false);

            if(rightResult == false) {
                return false;
            }

            if(leftResult.HasValue && rightResult.HasValue && leftResult.Value && rightResult.Value)
            {
                return true;
            }

            if(!forceMatch)
            {
                return null;
            }

            leftResult = leftResult ?? this.LeftChildNode.Match(rc, true);
            rightResult = rightResult ?? this.RightChildNode.Match(rc, true);

            if (!leftResult.HasValue)
            {
                throw new InvalidOperationException("Left child is null despite forced match");
            }

            if (!rightResult.HasValue)
            {
                throw new InvalidOperationException("Left child is null despite forced match");
            }

            return leftResult.Value && rightResult.Value;
        }

        public override string ToString()
        {
            return "(&:" + this.LeftChildNode + this.RightChildNode + ")";
        }

        #endregion
    }
}