namespace Stwalkerster.IrcClient.Events
{
    using System;

    /// <summary>
    /// The data received event args.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DataReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public DataReceivedEventArgs(string data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public string Data { get; private set; }
    }
}
