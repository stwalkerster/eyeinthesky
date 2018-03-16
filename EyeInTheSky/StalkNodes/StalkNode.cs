namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    public abstract class StalkNode : IStalkNode
    {
        public abstract bool Match(IRecentChange rc);
    }
}
