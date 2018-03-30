namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkConfiguration : IConfigurationBase<IStalk>
    {
        IEnumerable<IStalk> MatchStalks(IRecentChange rc);
    }
}