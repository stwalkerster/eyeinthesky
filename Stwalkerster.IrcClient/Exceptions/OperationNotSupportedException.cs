namespace Stwalkerster.IrcClient.Exceptions
{
    using System;

    /// <summary>
    /// The operation not supported exception.
    /// </summary>
    public class OperationNotSupportedException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ApplicationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error. </param>
        public OperationNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
