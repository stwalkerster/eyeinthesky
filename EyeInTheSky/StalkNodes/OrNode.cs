using System;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("or")]
    public class OrNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            var leftResult = this.LeftChildNode.Match(rc, false);

            if (leftResult == true)
            {
                return true;
            }

            var rightResult = this.RightChildNode.Match(rc, false);

            if (rightResult == true)
            {
                return true;
            }

            if (leftResult.HasValue && rightResult.HasValue && !leftResult.Value && !rightResult.Value)
            {
                return false;
            }

            if (!forceMatch)
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

            return leftResult.Value || rightResult.Value;
        }

        public override string ToString()
        {
            return "(|:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}