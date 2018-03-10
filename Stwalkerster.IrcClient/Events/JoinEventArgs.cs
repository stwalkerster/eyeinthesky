namespace Stwalkerster.IrcClient.Events
{
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Messages;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The join event args.
    /// </summary>
    public class JoinEventArgs : UserEventArgsBase
    {
        /// <summary>
        /// The channel.
        /// </summary>
        private readonly string channel;

        /// <summary>
        /// Initialises a new instance of the <see cref="JoinEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="user">
        ///     The user.
        /// </param>
        /// <param name="channel">
        ///     The channel.
        /// </param>
        /// <param name="client"></param>
        public JoinEventArgs(IMessage message, IUser user, string channel, IIrcClient client)
            : base(message, user, client)
        {
            this.channel = channel;
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }
    }
}