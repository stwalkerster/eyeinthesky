namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("not")]
    public class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return !this.ChildNode.Match(rc, forceMatch);
        }

        public override string ToString()
        {
            return "(!" + this.ChildNode +  ")";
        }
        #endregion
    }
}