namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IIrcChannel: INamedItem
    {
        List<ChannelUser> Users { get; }
        Dictionary<string, IStalk> Stalks { get; }
        Guid Guid { get; }
    }
}