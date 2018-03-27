namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("false")]
    public class FalseNode : LogicalNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return false;
        }

        public override string ToString()
        {
            return "(false)";
        }
    }
}