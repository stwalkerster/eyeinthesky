namespace EyeInTheSky.Model.StalkNodes.NumericNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("pagesize")]
    public class PageSizeNumberNode : NumberProviderNode
    {
        public override long? GetValue(IRecentChange rc, bool forceMatch)
        {
            if (!forceMatch)
            {
                return null;
            }

            return rc.GetPageSize();
        }

        public override string ToString()
        {
            return "(#pagesize)";
        }
    }
}