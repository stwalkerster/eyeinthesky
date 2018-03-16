namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkNode
    {
        bool Match(IRecentChange rc);
    }
}