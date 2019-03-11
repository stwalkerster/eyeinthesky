namespace EyeInTheSky.Model.StalkNodes.NumericNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    
    [StalkNodeType("number")]
    public class StaticNumberNode : NumberProviderNode
    {
        public override long? GetValue(IRecentChange rc, bool forceMatch)
        {
            return this.Value;
        }

        public override string ToString()
        {
            return string.Format("(number:{0})", this.Value);
        }

        public long Value { get; set; }
    }
}