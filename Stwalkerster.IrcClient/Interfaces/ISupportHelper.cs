namespace Stwalkerster.IrcClient.Interfaces
{
    using System.Collections.Generic;

    public interface ISupportHelper
    {
        /// <summary>
        /// The handle prefix message.
        /// </summary>
        /// <param name="prefixMessage">
        /// The prefix message.
        /// </param>
        /// <param name="prefixes">
        /// The prefix dictionary to modify
        /// </param>
        void HandlePrefixMessageSupport(string prefixMessage, IDictionary<string, string> prefixes);

        /// <summary>
        /// The handle status message.
        /// </summary>
        /// <param name="statusMessage">
        /// The status message.
        /// </param>
        /// <param name="supportedDestinationFlags">
        /// the supported destination flags list to modify
        /// </param>
        void HandleStatusMessageSupport(string statusMessage, IList<string> supportedDestinationFlags);
    }
}