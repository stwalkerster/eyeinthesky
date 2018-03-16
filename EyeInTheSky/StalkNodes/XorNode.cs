namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("xor")]
    public class XorNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool DoMatch(IRecentChange rc)
        {
            return this.LeftChildNode.Match(rc) ^ this.RightChildNode.Match(rc);
        }

        public override string ToString()
        {
            return "(^:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}