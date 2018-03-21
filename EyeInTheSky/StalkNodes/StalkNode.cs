namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Model;
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
            
            return this.Match(rc, true);
        }
        
        protected bool? Match(IRecentChange rc, bool forceMatch)
        {
            this.SanityCheck(rc);
            return this.DoMatch(rc, forceMatch);
        }
        
        protected abstract bool? DoMatch(IRecentChange rc, bool forceMatch);
    }
}
