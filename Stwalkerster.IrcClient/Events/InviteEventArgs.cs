namespace Stwalkerster.IrcClient.Events
{
    using Stwalkerster.IrcClient.Messages;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The invite event args.
    /// </summary>
    public class InviteEventArgs : UserEventArgsBase
    {
        /// <summary>
        /// The channel.
        /// </summary>
        private readonly string channel;

        /// <summary>
        /// The nickname.
        /// </summary>
        private readonly string nickname;

        /// <summary>
        /// Initialises a new instance of the <see cref="InviteEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="nickname">
        /// The nickname.
        /// </param>
        public InviteEventArgs(IMessage message, IUser user, string channel, string nickname)
            : base(message, user)
        {
            this.channel = channel;
            this.nickname = nickname;
        }

        /// <summary>
        ///     Gets the channel.
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }

        /// <summary>
        /// Gets the nickname.
        /// </summary>
        public string Nickname
        {
            get { return this.nickname; }
        }
    }
}