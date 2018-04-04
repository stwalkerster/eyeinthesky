using EyeInTheSky.Attributes;
using EyeInTheSky.Model.Interfaces;
using EyeInTheSky.Model.StalkNodes.BaseNodes;

namespace EyeInTheSky.Model.StalkNodes
{
    [StalkNodeType("log")]
    public class LogStalkNode : RegexLeafNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (rc.Log != null)
            {
                return this.RegexExpression.Match(rc.Log).Success;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "(log:\"" + this.RegexExpression + "\")";
        }
    }
}