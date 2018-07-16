namespace EyeInTheSky.Model.StalkNodes
{
    using Castle.Components.DictionaryAdapter;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("flag")]
    public class FlagStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            this.Compile();
            
            if (rc.EditFlags != null)
            {
                return this.RegexExpression.Match(rc.EditFlags).Success;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "(flag:\"" + this.rawRegexExpression + "\")";
        }
        #endregion
    }
}
