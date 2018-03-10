namespace EyeInTheSky.StalkNodes
{
    abstract class DoubleChildLogicalNode : LogicalNode
    {
        public StalkNode LeftChildNode { get; set; }
        public StalkNode RightChildNode { get; set; }
    }
}
