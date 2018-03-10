namespace Stwalkerster.IrcClient.Messages
{
    using System.Collections.Generic;

    public class Notice : Message
    {
        public Notice(string destination, string message)
            : base("NOTICE", new List<string> { destination, message })
        {
        }
    }
}