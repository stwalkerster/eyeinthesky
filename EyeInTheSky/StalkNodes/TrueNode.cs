namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

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
