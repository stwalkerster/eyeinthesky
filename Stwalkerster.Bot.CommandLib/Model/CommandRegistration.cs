namespace Stwalkerster.Bot.CommandLib.Model
{
    using System;

    internal class CommandRegistration
    {
        /// <summary>Initializes a new instance of the <see cref="T:CommandRegistration" /> class.</summary>
        public CommandRegistration(string channel, Type type)
        {
            this.Channel = channel;
            this.Type = type;
        }

        public string Channel { get; private set; }

        public Type Type { get; private set; }
    }
}