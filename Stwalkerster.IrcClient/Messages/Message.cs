namespace Stwalkerster.IrcClient.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Stwalkerster.IrcClient.Extensions;

    /// <summary>
    /// The message.
    /// </summary>
    public class Message : IMessage
    {
        /// <summary>
        /// The command.
        /// </summary>
        private readonly string command;

        /// <summary>
        /// The prefix.
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// The parameters.
        /// </summary>
        private readonly IEnumerable<string> parameters;

        /// <summary>
        /// Initialises a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public Message(string command)
            : this(null, command, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameter">
        /// The parameters.
        /// </param>
        public Message(string command, string parameter)
            : this(null, command, parameter.ToEnumerable())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public Message(string command, IEnumerable<string> parameters)
            : this(null, command, parameters)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public Message(string prefix, string command, IEnumerable<string> parameters)
        {
            this.prefix = prefix;
            this.command = command;
            this.parameters = parameters ?? new List<string>();
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        public string Prefix
        {
            get { return this.prefix; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<string> Parameters
        {
            get { return this.parameters == null ? null : this.parameters.ToArray(); }
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="IMessage" />.
        /// </returns>
        public static IMessage Parse(string data)
        {
            var separator = new[] {' '};
            
            // Define the parts of the message
            // It's always going to be an optional prefix (prefixed with a :), a command word, and 0 or more parameters to the command
            
            string prefix = null;
            string command;
            List<string> messageParameters = null;

            // Look for a prefix
            if (data.StartsWith(":"))
            {
                // Split the incoming data into a prefix and remainder
                var prefixstrings = data.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                
                // overwrite the original data, so we don't have to think about the prefix later.
                // This is now a command word, and 0 or more parameters to the command.
                data = prefixstrings[1];
                
                // Extract the prefix itself, stripping the leading : too.
                prefix = prefixstrings[0].Substring(1);
            }

            // Split out the command word
            var strings = data.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            command = strings[0];

            // strings is an array of {command, parameters}, unless there are no parameters to the command
            if (strings.Length == 2)
            {
                // This contains the entire string of parameters.
                var parameters = strings[1];

                string lastParam = null;
                
                if (parameters.StartsWith(":"))
                {
                    // The entire parameter string is a single parameter.
                    lastParam = parameters.Substring(1);
                    parameters = string.Empty;
                }
                
                if (parameters.Contains(" :"))
                {
                    // everything after this is a parameter. The +2 magic value == length of separator to exclude it
                    lastParam = parameters.Substring(parameters.IndexOf(" :", StringComparison.InvariantCulture) + 2);
                    parameters = parameters.Substring(0, parameters.IndexOf(" :", StringComparison.InvariantCulture));
                }
                
                messageParameters = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (lastParam != null)
                {
                    messageParameters.Add(lastParam);
                }
            }

            return new Message(prefix, command, messageParameters);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(this.Prefix))
            {
                result += ":" + this.Prefix + " ";
            }

            result += this.Command;

            foreach (var p in this.Parameters)
            {
                if (p.Contains(" "))
                {
                    result += " :" + p;
                }
                else
                {
                    result += " " + p;
                }
            }

            return result;
        }
    }
}