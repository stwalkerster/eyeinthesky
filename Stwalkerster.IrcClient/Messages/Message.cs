﻿namespace Stwalkerster.IrcClient.Messages
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
        /// The parameters.
        /// </summary>
        private readonly IEnumerable<string> parameters;

        /// <summary>
        /// The prefix.
        /// </summary>
        private readonly string prefix;

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
            string prefix = null, command;
            List<string> messageParameters = null;

            if (data.StartsWith(":"))
            {
                var prefixstrings = data.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                data = prefixstrings[1];
                prefix = prefixstrings[0].Substring(1); // strip the leading : too
            }

            var strings = data.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            command = strings[0];

            if (strings.Length == 2)
            {
                var parameters = strings[1];

                if (parameters.Contains(" :") || parameters.StartsWith(":"))
                {
                    var paramend = parameters.Substring(parameters.IndexOf(":", StringComparison.Ordinal) + 1);
                    var parameterList =
                        parameters.Substring(0, parameters.IndexOf(":", StringComparison.Ordinal))
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                    parameterList.Add(paramend);
                    messageParameters = parameterList;
                }
                else
                {
                    messageParameters = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
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