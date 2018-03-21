namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    
    [StalkNodeType("flag")]
    public class FlagStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.RegexExpression.Match(rc.EditFlags).Success;
        }
        
        public override string ToString()
        {
            return "(flag:\"" + this.RegexExpression + "\")";
        }
        #endregion
    }
}
