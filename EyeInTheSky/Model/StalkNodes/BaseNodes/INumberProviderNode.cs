namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System;

    using EyeInTheSky.Model.Interfaces;

    public interface INumberProviderNode : ITreeNode
    {
        long? GetValue(IRecentChange rc, bool forceMatch);
    }
}