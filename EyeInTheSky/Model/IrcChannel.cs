namespace EyeInTheSky.Model
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public class IrcChannel : IIrcChannel
    {
        public IrcChannel(string channel, Guid guid)
        {
            this.Identifier = channel;
            this.Guid = guid;
            this.Users = new List<ChannelUser>();
            this.Stalks = new Dictionary<string, IStalk>();
        }

        public IrcChannel(string channel) : this(channel, Guid.NewGuid())
        {
        }

        public string Identifier { get; private set; }

        public List<ChannelUser> Users { get; private set; }
        public Dictionary<string, IStalk> Stalks { get; private set; }

        public Guid Guid { get; private set; }
    }
}