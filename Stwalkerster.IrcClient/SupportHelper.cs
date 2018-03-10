namespace Stwalkerster.IrcClient
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.IrcClient.Interfaces;

    /// <summary>
    /// The support helper.
    /// </summary>
    public class SupportHelper : ISupportHelper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportHelper"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public SupportHelper(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// The handle prefix message.
        /// </summary>
        /// <param name="prefixMessage">
        /// The prefix message.
        /// </param>
        /// <param name="prefixes">
        /// The prefix dictionary to modify
        /// </param>
        public void HandlePrefixMessageSupport(string prefixMessage, IDictionary<string, string> prefixes)
        {
            //// PREFIX=(Yqaohv)!~&@%+
            var strings = prefixMessage.Split('(', ')');
            var modes = strings[1];
            var symbols = strings[2];
            if (modes.Length != symbols.Length)
            {
                this.logger.ErrorFormat("RPL_ISUPPORT PREFIX not valid: {0}", prefixMessage);
                return;
            }

            for (var i = 0; i < modes.Length; i++)
            {
                prefixes.Add(modes.Substring(i, 1), symbols.Substring(i, 1));
            }
        }

        /// <summary>
        /// The handle status message.
        /// </summary>
        /// <param name="statusMessage">
        /// The status message.
        /// </param>
        /// <param name="supportedDestinationFlags">
        /// the supported destination flags list to modify
        /// </param>
        public void HandleStatusMessageSupport(string statusMessage, IList<string> supportedDestinationFlags)
        {
            //// STATUSMSG=@+
            var strings = statusMessage.Split('=');
            var modes = strings[1];

            for (var i = 0; i < modes.Length; i++)
            {
                supportedDestinationFlags.Add(modes.Substring(i, 1));
            }
        }
    }
}
