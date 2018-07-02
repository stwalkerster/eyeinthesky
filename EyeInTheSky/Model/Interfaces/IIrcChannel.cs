namespace EyeInTheSky.Model.Interfaces
{
    using System.Collections.Generic;

    public interface IIrcChannel: INamedItem
    {
        List<ChannelUser> Users { get; }
        Dictionary<string, IStalk> Stalks { get; }
    }
}