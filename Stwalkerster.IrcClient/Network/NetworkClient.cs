namespace Stwalkerster.IrcClient.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Castle.Core.Logging;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;

    /// <summary>
    ///     The TCP client.
    /// </summary>
    /// <para>
    ///     This is an event-based asynchronous TCP client
    /// </para>
    public class NetworkClient : INetworkClient
    {
        #region Public Events

        /// <summary>
        ///     The data received.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        #endregion

        #region Fields

        /// <summary>
        ///     The client.
        /// </summary>
        private readonly TcpClient client;

        /// <summary>
        ///     The network logger.
        /// </summary>
        private readonly ILogger inboundLogger;

        /// <summary>
        ///     The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        ///     The outbound logger.
        /// </summary>
        private readonly ILogger outboundLogger;

        /// <summary>
        ///     The send queue.
        /// </summary>
        private readonly Queue<string> sendQueue;

        /// <summary>
        ///     The send queue lock.
        /// </summary>
        private readonly object sendQueueLock = new object();

        /// <summary>
        ///     The reset event.
        /// </summary>
        private readonly AutoResetEvent writerThreadResetEvent;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initialises a new instance of the <see cref="NetworkClient" /> class.
        /// </summary>
        /// <param name="hostname">
        ///     The hostname.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        public NetworkClient(string hostname, int port, ILogger logger)
            : this(hostname, port, logger, true)
        {
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="NetworkClient" /> class.
        /// </summary>
        /// <param name="hostname">
        ///     The hostname.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="logger">
        ///     The logger.
        /// </param>
        /// <param name="startThreads">
        ///     The start threads.
        /// </param>
        protected NetworkClient(string hostname, int port, ILogger logger, bool startThreads)
        {
            this.Hostname = hostname;
            this.Port = port;
            this.logger = logger;
            this.inboundLogger = logger.CreateChildLogger("Inbound");
            this.outboundLogger = logger.CreateChildLogger("Outbound");

            this.logger.InfoFormat("Connecting to socket tcp://{0}:{1}/ ...", hostname, port);

            this.client = new TcpClient(this.Hostname, this.Port);

            this.Reader = new StreamReader(this.client.GetStream());
            this.Writer = new StreamWriter(this.client.GetStream());
            this.sendQueue = new Queue<string>();

            this.writerThreadResetEvent = new AutoResetEvent(true);

            if (startThreads)
            {
                this.StartThreads();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the client.
        /// </summary>
        public TcpClient Client
        {
            get { return this.client; }
        }

        public bool Connected
        {
            get { return this.client.Connected; }
        }

        /// <summary>
        ///     The hostname.
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        ///     The port.
        /// </summary>
        public int Port { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the reader.
        /// </summary>
        protected StreamReader Reader { get; set; }

        /// <summary>
        ///     Gets or sets the writer.
        /// </summary>
        protected StreamWriter Writer { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.logger.Info("Disconnecting network socket.");
            this.Writer.Flush();
            this.Writer.Close();
            this.client.Close();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The send.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void Send(string message)
        {
            lock (this.sendQueueLock)
            {
                this.sendQueue.Enqueue(message);
            }

            this.writerThreadResetEvent.Set();
        }

        /// <summary>
        ///     The send.
        /// </summary>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        public void Send(IEnumerable<string> messages)
        {
            lock (this.sendQueueLock)
            {
                foreach (var message in messages)
                {
                    this.sendQueue.Enqueue(message);
                }
            }

            this.writerThreadResetEvent.Set();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Reader.Dispose();
                this.Writer.Dispose();
                ((IDisposable) this.writerThreadResetEvent).Dispose();
                this.client.Close();
            }
        }

        /// <summary>
        ///     The on data received.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            var handler = this.DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        ///     The start threads.
        /// </summary>
        protected void StartThreads()
        {
            var readerThread = new Thread(this.ReaderThreadTask);
            var writerThread = new Thread(this.WriterThreadTask);

            this.logger.InfoFormat("Initialising reader/writer threads");

            readerThread.Start();
            writerThread.Start();
        }

        /// <summary>
        ///     The reader thread task.
        /// </summary>
        private void ReaderThreadTask()
        {
            while (this.client.Connected)
            {
                try
                {
                    var data = this.Reader.ReadLine();

                    if (data != null)
                    {
                        this.inboundLogger.Debug(data);
                        this.OnDataReceived(new DataReceivedEventArgs(data));
                    }
                }
                catch (IOException ex)
                {
                    this.logger.Error("IO error on read from network stream", ex);
                }
            }
        }

        /// <summary>
        ///     The writer thread task.
        /// </summary>
        private void WriterThreadTask()
        {
            while (this.client.Connected)
            {
                string item = null;

                // grab an item from the queue if we can
                lock (this.sendQueueLock)
                {
                    if (this.sendQueue.Count > 0)
                    {
                        item = this.sendQueue.Dequeue();
                    }
                }

                if (item == null)
                {
                    // Wait here for an item to be added to the queue
                    this.writerThreadResetEvent.WaitOne();
                }
                else
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }

                    this.outboundLogger.Debug(item);
                    this.Writer.WriteLine(item);
                    this.Writer.Flush();

                    // Flood protection
                    Thread.Sleep(500);
                }
            }
        }

        #endregion
    }
}