namespace EyeInTheSky.StalkNodes
{
    abstract class DoubleChildLogicalNode : LogicalNode
    {
        public IStalkNode LeftChildNode { get; set; }
        public IStalkNode RightChildNode { get; set; }
    }
}
