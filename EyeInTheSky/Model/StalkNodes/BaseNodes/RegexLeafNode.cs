namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;
    using System.Text.RegularExpressions;
    using EyeInTheSky.Model.Interfaces;

    public abstract class RegexLeafNode : LeafNode
    {
        protected Regex RegexExpression;

        public override void SetMatchExpression(string regex)
        {
            this.RegexExpression = new Regex(regex);
            base.SetMatchExpression(regex);
        }
        
        public override string GetMatchExpression()
        {
            return this.RegexExpression.ToString();
        }

        protected override void SanityCheck(IRecentChange rc)
        {
            base.SanityCheck(rc);

            if (this.RegexExpression == null)
            {
                throw new InvalidOperationException("No regex match expression has been set!");
            }
        }
    }
}
