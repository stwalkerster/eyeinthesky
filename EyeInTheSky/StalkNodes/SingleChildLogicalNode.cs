namespace EyeInTheSky.StalkNodes
{
    abstract class SingleChildLogicalNode : LogicalNode
    {
        public StalkNode ChildNode { get; set; }
    }
}
