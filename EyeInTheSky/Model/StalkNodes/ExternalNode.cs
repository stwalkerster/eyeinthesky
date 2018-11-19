namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("external")]
    public class ExternalNode : SingleChildLogicalNode
    {
        public string Provider { get; set; }
        public string Location { get; set; }
        
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.ChildNode.Match(rc, forceMatch);
        }

        public override string ToString()
        {
            return "(ext:" + this.ChildNode +  ")";
        }
    }
}