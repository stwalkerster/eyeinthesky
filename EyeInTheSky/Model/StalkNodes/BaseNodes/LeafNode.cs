namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public abstract class LeafNode : StalkNode
    {
        protected string Expression;

        public virtual void SetMatchExpression(string matchExpr)
        {
            this.Expression = matchExpr;
        }
        
        public virtual string GetMatchExpression()
        {
            return this.Expression;
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