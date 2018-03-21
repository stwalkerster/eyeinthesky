namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("user")]
    public class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.Expression.Match(rc.User).Success;
        }

        public override string ToString()
        {
            return "(user:\"" + this.Expression + "\")";
        }

        #endregion
    }
}