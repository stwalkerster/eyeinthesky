namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("actinguser")]
    public class ActingUserStalkNode : RegexLeafNode
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
            
            return result;
        }

        public override string ToString()
        {
            return "(actinguser:\"" + this.rawRegexExpression + "\")";
        }

        #endregion
    }
}