namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkNode : ICloneable
    {
        bool Match(IRecentChange rc);
        bool? Match(IRecentChange rc, bool forceMatch);
    }
}