namespace Stwalkerster.Bot.CommandLib.Exceptions
{
    using System;

    /// <summary>
    /// The command invocation exception.
    /// </summary>
    [Serializable]
    public class CommandInvocationException : CommandExecutionException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandInvocationException"/> class.
        /// </summary>
        public CommandInvocationException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandInvocationException"/> class.
        /// </summary>
        /// <param name="helpKey">
        /// The help key.
        /// </param>
        public CommandInvocationException(string helpKey)
        {
            this.HelpKey = helpKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the help key.
        /// </summary>
        public string HelpKey { get; private set; }

        #endregion
    }
}