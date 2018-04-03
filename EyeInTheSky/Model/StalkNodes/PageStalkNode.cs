namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("page")]
    public class PageStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.RegexExpression.Match(rc.Page).Success || this.RegexExpression.Match(rc.TargetPage).Success;
        }

        public override string ToString()
        {
            return "(page:\"" + this.RegexExpression + "\")";
        }

        #endregion
    }
}