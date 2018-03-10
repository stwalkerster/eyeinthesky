namespace Stwalkerster.Bot.CommandLib.Exceptions
{
    using System;

    /// <summary>
    /// The command error exception.
    /// </summary>
    [Serializable]
    public class CommandErrorException : CommandExecutionException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandErrorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        public CommandErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandErrorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public CommandErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}