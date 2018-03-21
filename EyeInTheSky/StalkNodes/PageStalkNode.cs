namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("page")]
    public class PageStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.RegexExpression.Match(rc.Page).Success;
        }

        public override string ToString()
        {
            return "(page:\"" + this.RegexExpression + "\")";
        }

        #endregion
    }
}