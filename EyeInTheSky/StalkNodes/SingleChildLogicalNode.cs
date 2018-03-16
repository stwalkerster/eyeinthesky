namespace EyeInTheSky.StalkNodes
{
    abstract class SingleChildLogicalNode : LogicalNode
    {
        public IStalkNode ChildNode { get; set; }
    }
}
