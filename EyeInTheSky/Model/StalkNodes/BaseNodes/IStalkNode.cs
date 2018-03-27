namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkNode
    {
        bool Match(IRecentChange rc);
        bool? Match(IRecentChange rc, bool forceMatch);
    }
}