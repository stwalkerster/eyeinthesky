namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    
    [StalkNodeType("flag")]
    class FlagStalkNode : LeafNode
    {
        #region Overrides of StalkNode

        public override bool Match(IRecentChange rc)
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
