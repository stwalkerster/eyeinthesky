namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("summary")]
    public class SummaryStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (rc.EditSummary != null)
            {
                return this.RegexExpression.Match(rc.EditSummary).Success;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "(summary:\"" + this.RegexExpression + "\")";
        }
        #endregion
    }
}