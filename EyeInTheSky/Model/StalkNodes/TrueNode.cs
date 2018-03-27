namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("true")]
    public class TrueNode : LogicalNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return true;
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}
