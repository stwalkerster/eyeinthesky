namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("summary")]
    class SummaryStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
        {
            return this.Expression.Match(rc.EditSummary).Success;
        }
        
        public override string ToString()
        {
            return "(summary:\"" + this.Expression + "\")";
        }
        #endregion
    }
}