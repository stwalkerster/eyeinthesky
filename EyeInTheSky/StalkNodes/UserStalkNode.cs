namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("user")]
    public class UserStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.RegexExpression.Match(rc.User).Success;
        }

        public override string ToString()
        {
            return "(user:\"" + this.RegexExpression + "\")";
        }

        #endregion
    }
}