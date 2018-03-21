namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("page")]
    public class PageStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.Expression.Match(rc.Page).Success;
        }

        public override string ToString()
        {
            return "(page:\"" + this.Expression + "\")";
        }

        #endregion
    }
}