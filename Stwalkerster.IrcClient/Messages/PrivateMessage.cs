namespace Stwalkerster.IrcClient.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// The private message.
    /// </summary>
    public class PrivateMessage : Message
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="PrivateMessage"/> class.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public PrivateMessage(string destination, string message)
            : base("PRIVMSG", new List<string> { destination, message })
        {
        }

        #endregion
    }
}