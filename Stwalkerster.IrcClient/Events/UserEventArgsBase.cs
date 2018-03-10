namespace Stwalkerster.IrcClient.Events
{
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Messages;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The user event args base.
    /// </summary>
    public class UserEventArgsBase : MessageReceivedEventArgs
    {
        /// <summary>
        /// The user.
        /// </summary>
        private readonly IUser user;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserEventArgsBase"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        public UserEventArgsBase(IMessage message, IUser user, IIrcClient client)
            : base(message, client)
        {
            this.user = user;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public IUser User
        {
            get
            {
                return this.user;
            }
        }
    }
}