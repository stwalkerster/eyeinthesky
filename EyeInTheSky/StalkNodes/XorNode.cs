namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

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

            if(leftResult.HasValue && rightResult.HasValue && leftResult.Value != rightResult.Value)
            {
                return true;
            }

            if(!forceMatch)
            {
                return null;
            }
            
            return this.LeftChildNode.Match(rc, true) ^ this.RightChildNode.Match(rc, true);
        }

        public override string ToString()
        {
            return "(^:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}