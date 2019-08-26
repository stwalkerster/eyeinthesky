namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkNode : ITreeNode
    {
        bool Match(IRecentChange rc);
        bool? Match(IRecentChange rc, bool forceMatch);

        string Comment { get; set; }
    }
}