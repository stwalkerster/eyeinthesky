namespace EyeInTheSky.Model.StalkNodes
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("x-of")]
    public class XOfStalkNode : MultiChildLogicalNode
    {
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            int maxLimit = 0;
            int minLimit = 0;

            foreach (var node in this.ChildNodes)
            {
                var localResult = node.Match(rc, forceMatch);

                if (!localResult.HasValue)
                {
                    maxLimit++;
                    continue;
                }
                
                if (localResult.Value)
                {
                    maxLimit++;
                    minLimit++;
                }
            }

            if (this.Maximum.HasValue && minLimit > this.Maximum)
            {
                return false;
            }

            if (this.Minimum.HasValue && maxLimit < this.Minimum)
            {
                return false;
            }

            if (minLimit < this.Minimum || maxLimit > this.Maximum)
            {
                return null;
            }

            return true;
        }

        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);

            if (!this.Minimum.HasValue && !this.Maximum.HasValue)
            {
                throw new InvalidOperationException("Must define at least one of min or max");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("(#[");
            sb.Append(this.Minimum.HasValue ? this.Minimum.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            sb.Append(",");
            sb.Append(this.Maximum.HasValue ? this.Maximum.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            sb.Append("]:");
            
            foreach (var node in this.ChildNodes)
            {
                sb.Append(node);
            }
            
            sb.Append(")");
            
            return sb.ToString(); 
        }
    }
}