namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    
    [StalkNodeType("flag")]
    public class FlagStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.Expression.Match(rc.EditFlags).Success;
        }
        
        public override string ToString()
        {
            return "(flag:\"" + this.Expression + "\")";
        }
        #endregion
    }
}
