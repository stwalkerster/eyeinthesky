namespace EyeInTheSky.Model
{
    using Stwalkerster.IrcClient.Model;

    public class StalkUser
    {
        public StalkUser(IrcUserMask mask)
        {
            this.Mask = mask;
        }

        public IrcUserMask Mask { get; private set; }
    }
}