namespace Stwalkerster.IrcClient.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// The Message interface.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        IEnumerable<string> Parameters { get; }
    }
}