namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;

    using EyeInTheSky.Model.Interfaces;

    public abstract class NumberProviderNode :INumberProviderNode
    {
        public object Clone()
        {
            var clone = this.MemberwiseClone();
            this.PopulateClone((INumberProviderNode) clone);
            return clone;
        }

        public abstract long? GetValue(IRecentChange rc, bool forceMatch);

        protected virtual void PopulateClone(INumberProviderNode node)
        {
        }
    }
}