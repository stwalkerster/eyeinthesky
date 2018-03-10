namespace Stwalkerster.Bot.CommandLib.Exceptions
{
    using System;

    /// <summary>
    /// The configuration exception.
    /// </summary>
    [Serializable]
    public class ConfigurationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ConfigurationException(string message)
            : base(message)
        {
        }

        #endregion
    }
}