namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response
{
    using System.Collections.Generic;
    using System.Linq;
    using Stwalkerster.Extensions;
    using Stwalkerster.IrcClient.Extensions;

    /// <summary>
    /// The individual response
    /// </summary>
    public class CommandResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the client to client protocol.
        /// </summary>
        public string ClientToClientProtocol { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public CommandResponseDestination Destination { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the directed to.
        /// </summary>
        public IEnumerable<string> RedirectionTarget { get; set; }

        /// <summary>
        /// The type of response
        /// </summary>
        public CommandResponseType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore redirection.
        /// </summary>
        public bool IgnoreRedirection { get; set; }

        #endregion

        /// <summary>
        /// The compile message.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CompileMessage()
        {
            string message = this.Message;

            if (this.ClientToClientProtocol != null)
            {
                message = message.SetupForCtcp(this.ClientToClientProtocol);
            }
            else
            {
                if (!this.IgnoreRedirection && this.RedirectionTarget != null && this.RedirectionTarget.Any())
                {
                    message = string.Format("{0}: {1}", this.RedirectionTarget.Implode(", "), message);
                }
            }

            return message;
        }
    }
}