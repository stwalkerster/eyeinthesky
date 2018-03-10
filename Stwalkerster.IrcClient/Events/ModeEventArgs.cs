namespace Stwalkerster.IrcClient.Events
{
    using System.Collections.Generic;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Messages;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class ModeEventArgs : UserEventArgsBase
    {
        public ModeEventArgs(IMessage message,
            IUser user,
            string target,
            IEnumerable<string> changes,
            IIrcClient client) : base(
            message,
            user,
            client)
        {
            this.Target = target;
            this.Changes = changes;
        }

        public string Target { get; private set; }
        public IEnumerable<string> Changes { get; private set; }
    }
}