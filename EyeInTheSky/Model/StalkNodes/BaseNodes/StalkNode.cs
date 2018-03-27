namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public abstract class StalkNode : IStalkNode
    {
        protected virtual void SanityCheck(IRecentChange rc)
        {
            if (rc == null)
            {
                throw new ArgumentNullException("rc");
            }
        }

        public bool Match(IRecentChange rc)
        {
            var initialResult = this.Match(rc, false);
            if(initialResult.HasValue) {
                return initialResult.Value;
            }

            var match = this.Match(rc, true);
            if (match.HasValue)
            {
                return match.Value;
            }
            
            throw new InvalidOperationException("Result of forced match is null!");
        }
        
        public bool? Match(IRecentChange rc, bool forceMatch)
        {
            this.SanityCheck(rc);
            return this.DoMatch(rc, forceMatch);
        }
        
        protected abstract bool? DoMatch(IRecentChange rc, bool forceMatch);
    }
}
