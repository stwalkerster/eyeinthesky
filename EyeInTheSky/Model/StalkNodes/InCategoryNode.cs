namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("incategory")]
    public class InCategoryNode : LeafNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (!forceMatch)
            {
                return null;
            }

            return rc.PageIsInCategory(this.Expression);
        }

        public override string ToString()
        {
            return string.Format("(incat:{0})", this.Expression);
        }
    }
}