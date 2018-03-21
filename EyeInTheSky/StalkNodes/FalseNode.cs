namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

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