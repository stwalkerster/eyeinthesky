namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("user")]
    public class UserStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            this.Compile();
            
            var result = false;
            
            if (rc.User != null)
            {
                result |= this.RegexExpression.Match(rc.User).Success;
            }
            
            if (rc.TargetUser != null)
            {
                result |= this.RegexExpression.Match(rc.TargetUser).Success;
            }

            return result;
        }

        public override string ToString()
        {
            return "(user:\"" + this.rawRegexExpression + "\")";
        }

        #endregion
    }
}