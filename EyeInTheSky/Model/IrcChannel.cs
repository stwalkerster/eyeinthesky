namespace EyeInTheSky.Model
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public class IrcChannel : IIrcChannel
    {
        public IrcChannel(string channel)
        {
            this.Identifier = channel;
            this.Users = new List<ChannelUser>();
            this.Stalks = new Dictionary<string, IStalk>();
        }

        public string Identifier { get; private set; }

        public List<ChannelUser> Users { get; private set; }
        public Dictionary<string, IStalk> Stalks { get; private set; }
    }
}