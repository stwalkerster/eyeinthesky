namespace EyeInTheSky.Model
{
    using Stwalkerster.IrcClient.Model;

    public class StalkUser
    {
        public StalkUser(IrcUserMask mask, bool subscribed)
        {
            this.Mask = mask;
            this.Subscribed = subscribed;
        }

        public IrcUserMask Mask { get; private set; }
        
        public bool Subscribed { get; set; }
    }
}