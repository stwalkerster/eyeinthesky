namespace EyeInTheSky.Model
{
    using Stwalkerster.IrcClient.Model;

    public class ChannelUser
    {
        public ChannelUser(IrcUserMask mask)
        {
            this.Mask = mask;
        }

        public IrcUserMask Mask { get; private set; }
        public string LocalFlags { get; set; }
        public bool Subscribed { get; set; }
    }
}