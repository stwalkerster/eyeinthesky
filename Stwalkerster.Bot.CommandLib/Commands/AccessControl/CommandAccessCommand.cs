namespace Stwalkerster.Bot.CommandLib.Commands.AccessControl
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The command access command.
    /// </summary>
    [CommandInvocation("commandaccess")]
    [CommandFlag(Model.Flag.Standard)]
    public class CommandAccessCommand : CommandBase
    {
        #region Fields

        /// <summary>
        /// The command parser.
        /// </summary>
        private readonly ICommandParser commandParser;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandAccessCommand"/> class. 
        /// </summary>
        /// <param name="commandSource">
        /// The command source.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="flagService">
        /// </param>
        /// <param name="configProvider">
        /// </param>
        /// <param name="commandParser">
        /// The command Parser.
        /// </param>
        /// <param name="client"></param>
        public CommandAccessCommand(
            string commandSource, 
            IUser user, 
            IEnumerable<string> arguments, 
            ILogger logger, 
            IFlagService flagService, 
            IConfigurationProvider configProvider, 
            ICommandParser commandParser,
            IIrcClient client)
            : base(commandSource, user, arguments, logger, flagService, configProvider, client)
        {
            this.commandParser = commandParser;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        protected override IEnumerable<CommandResponse> Execute()
        {
            if (!this.Arguments.Any())
            {
                throw new ArgumentCountException(1, this.Arguments.Count());
            }

            var command = this.commandParser.GetCommand(
                new CommandMessage { CommandName = this.Arguments.First() }, 
                this.User, 
                this.CommandSource, 
                this.Client);

            var message = string.Format("The command {0} requires the flag '{1}'.", this.Arguments.First(), command.Flag);
            yield return new CommandResponse { Message = message };
        }

        #endregion
    }
}