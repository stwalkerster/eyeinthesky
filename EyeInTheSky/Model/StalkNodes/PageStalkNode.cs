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
            var result = false;
            
            if (rc.Page != null)
            {
                result |= this.RegexExpression.Match(rc.Page).Success;
            }
            if (rc.TargetPage != null)
            {
                result |= this.RegexExpression.Match(rc.TargetPage).Success;
            }

            return result;
        }

        public override string ToString()
        {
            return "(page:\"" + this.RegexExpression + "\")";
        }

        #endregion
    }
}