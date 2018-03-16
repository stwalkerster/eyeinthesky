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
            this.SanityCheck(rc);

            return this.DoMatch(rc);
        }
        
        protected abstract bool DoMatch(IRecentChange rc);
    }
}
