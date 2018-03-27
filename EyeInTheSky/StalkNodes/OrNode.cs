using System;

namespace EyeInTheSky.StalkNodes
{
    using System.Text;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("or")]
    public class OrNode : MultiChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            // Pessimism!
            bool? result = false;

            foreach (var childNode in this.ChildNodes)
            {
                var localResult = childNode.Match(rc, forceMatch);

                if (localResult.GetValueOrDefault(false))
                {
                    return true;
                }

                if (localResult != null)
                {
                    continue;
                }

                if (forceMatch)
                {
                    throw new InvalidOperationException("Child is null despite forced match");
                }
                    
                result = null;
            }

            return result;        
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            sb.Append("(|:");
            
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