namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public interface IChannelConfiguration : IConfigurationBase<IIrcChannel>
    {
        IEnumerable<IStalk> MatchStalks(IRecentChange rc, string channel);
    }
}