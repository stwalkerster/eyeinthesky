namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("targetuser")]
    public class TargetUserStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            this.Compile();
            
            var result = false;
            
            if (rc.TargetUser != null)
            {
                result |= this.RegexExpression.Match(rc.TargetUser).Success;
            }

            return result;
        }

        public override string ToString()
        {
            return "(targetuser:\"" + this.rawRegexExpression + "\")";
        }

        #endregion
    }
}