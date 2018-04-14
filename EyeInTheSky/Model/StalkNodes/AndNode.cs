namespace EyeInTheSky.Model.StalkNodes
{
    using System;
    using System.Text;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("and")]
    public class AndNode : MultiChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            // Optimism!
            bool? result = true;

            foreach (var childNode in this.ChildNodes)
            {
                var localResult = childNode.Match(rc, forceMatch);

                if (localResult.GetValueOrDefault(true) == false)
                {
                    return false;
                }

                if (localResult != null)
                {
                    continue;
                }
                    
                result = null;
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.Append("(&:");
            
            foreach (var node in this.ChildNodes)
            {
                sb.Append(node);
            }
            
            sb.Append(")");
            
            
            return sb.ToString();
        }

        #endregion
    }
}