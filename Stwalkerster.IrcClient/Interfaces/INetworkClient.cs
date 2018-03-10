namespace Stwalkerster.IrcClient.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Stwalkerster.IrcClient.Events;

    /// <summary>
    /// The NetworkClient interface.
    /// </summary>
    public interface INetworkClient : IDisposable
    {
        bool Connected { get; }

        /// <summary>
        /// The data received.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Send(string message);

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        void Send(IEnumerable<string> messages);

        /// <summary>
        /// The disconnect.
        /// </summary>
        void Disconnect();
    }
}