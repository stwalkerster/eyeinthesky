namespace EyeInTheSky.Services.RecentChanges.EventStreams
{
    using System;
    using Castle.Core;
    using Castle.Core.Logging;
    using EyeInTheSky.Services.RecentChanges.Interfaces;
    using LaunchDarkly.EventSource;

    public class EventStreamClient : IStartable, IDisposable
    {
        private readonly ILogger logger;
        private readonly IRecentChangeHandler rcHandler;
        private EventSource eventSource;
        private Uri sourceUri;

        public EventStreamClient(ILogger logger, IRecentChangeHandler rcHandler)
        {
            this.logger = logger;
            this.rcHandler = rcHandler;

            this.sourceUri = new Uri("https://stream.wikimedia.org/v2/stream/recentchange");
        }

        private void EventSourceOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.logger.Debug(e.Message.Data);
        }

        public void Start()
        {
            var configuration = new Configuration(this.sourceUri);
            this.eventSource = new EventSource(configuration);
            
            this.eventSource.MessageReceived += this.EventSourceOnMessageReceived;
            
            this.logger.Warn("Starting EventStreams");
            this.eventSource.StartAsync().Wait();
        }

        public void Stop()
        {
            this.logger.Warn("Stopping EventStreams");
            this.eventSource.Close();
        }

        public void Dispose()
        {
            if (this.eventSource != null) this.eventSource.Dispose();
        }
    }
}