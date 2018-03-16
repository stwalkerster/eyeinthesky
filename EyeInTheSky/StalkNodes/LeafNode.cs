using System.Text.RegularExpressions;

namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public abstract class LeafNode : StalkNode
    {
        protected Regex Expression;

        public void SetMatchExpression(string regex)
        {
            this.Expression = new Regex(regex);
        }
        
        public string GetMatchExpression()
        {
            return this.Expression.ToString();
        }

        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);

            if (this.Expression == null)
            {
                throw new InvalidOperationException("No match expression has been set!");
            }
        }
    }
}
