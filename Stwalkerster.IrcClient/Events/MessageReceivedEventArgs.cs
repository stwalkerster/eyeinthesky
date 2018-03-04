namespace Stwalkerster.IrcClient.Events
{
    using System;
    using Stwalkerster.IrcClient.Messages;

    /// <summary>
    /// The message received event args.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The message.
        /// </summary>
        private readonly IMessage message;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public MessageReceivedEventArgs(IMessage message)
        {
            this.message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public IMessage Message
        {
            get { return this.message; }
        }
    }
}