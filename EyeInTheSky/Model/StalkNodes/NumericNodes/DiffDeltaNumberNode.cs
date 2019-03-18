namespace EyeInTheSky.Model.StalkNodes.NumericNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("diffsize")]
    public class DiffDeltaNumberNode : NumberProviderNode
    {
        public override long? GetValue(IRecentChange rc, bool forceMatch)
        {
            return rc.SizeDiff;
        }

        public override string ToString()
        {
            return "(#diffsize)";
        }
    }
}