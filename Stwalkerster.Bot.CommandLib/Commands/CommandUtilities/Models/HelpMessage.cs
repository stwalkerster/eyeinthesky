namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.IrcClient.Extensions;

    /// <summary>
    /// The help message.
    /// </summary>
    public class HelpMessage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpMessage"/> class. 
        /// </summary>
        /// <param name="commandName">
        /// The command Name.
        /// </param>
        /// <param name="syntax">
        /// The syntax.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public HelpMessage(string commandName, string syntax, string text)
            : this(commandName, syntax.ToEnumerable(), text.ToEnumerable())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpMessage"/> class.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <param name="syntax">
        /// The syntax.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public HelpMessage(string commandName, IEnumerable<string> syntax, string text)
            : this(commandName, syntax, text.ToEnumerable())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpMessage"/> class.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <param name="syntax">
        /// The syntax.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public HelpMessage(string commandName, string syntax, IEnumerable<string> text)
            : this(commandName, syntax.ToEnumerable(), text)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpMessage"/> class.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <param name="syntax">
        /// The syntax.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public HelpMessage(string commandName, IEnumerable<string> syntax, IEnumerable<string> text)
        {
            this.CommandName = commandName;
            this.Syntax = syntax;
            this.Text = text;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the syntax.
        /// </summary>
        public IEnumerable<string> Syntax { get; private set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public IEnumerable<string> Text { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The to command responses.
        /// </summary>
        /// <param name="commandTrigger">
        /// The command Trigger.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        public IEnumerable<CommandResponse> ToCommandResponses(string commandTrigger)
        {
            var messages = new List<CommandResponse>();

            messages.AddRange(
                this.Syntax.Select(
                    syntax =>
                    new CommandResponse
                        {
                            Message =
                                string.Format("{2}{0} {1}", this.CommandName, syntax, commandTrigger), 
                            Destination = CommandResponseDestination.PrivateMessage
                        }));

            messages.AddRange(
                this.Text.Select(
                    helpText =>
                    new CommandResponse
                        {
                            Message = string.Format("   {0}", helpText), 
                            Destination = CommandResponseDestination.PrivateMessage
                        }));

            return messages;
        }

        #endregion
    }
}