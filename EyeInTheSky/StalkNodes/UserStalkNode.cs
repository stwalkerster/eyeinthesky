namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("user")]
    class UserStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
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
