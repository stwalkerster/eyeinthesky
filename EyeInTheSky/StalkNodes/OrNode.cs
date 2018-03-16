namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("or")]
    class OrNode : DoubleChildLogicalNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.LeftChildNode.Match(rc) || this.RightChildNode.Match(rc);
        }

        public override string ToString()
        {
            return "(|:" + this.LeftChildNode + this.RightChildNode + ")";
        }
        #endregion
    }
}