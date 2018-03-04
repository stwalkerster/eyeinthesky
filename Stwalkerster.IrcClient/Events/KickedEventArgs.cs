namespace Stwalkerster.IrcClient.Events
{
    using System;

    public class KickedEventArgs : EventArgs
    {
        public KickedEventArgs(string channel)
        {
            this.Channel = channel;
        }

        public string Channel { get; private set; }
    }
}