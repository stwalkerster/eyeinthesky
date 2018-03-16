namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("page")]
    class PageStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
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