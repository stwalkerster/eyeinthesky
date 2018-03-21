using System;
using EyeInTheSky.Model.Interfaces;

namespace EyeInTheSky.StalkNodes
{
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