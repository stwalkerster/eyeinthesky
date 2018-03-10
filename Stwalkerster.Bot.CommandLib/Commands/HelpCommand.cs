namespace Stwalkerster.Bot.CommandLib.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Extensions;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The help command.
    /// </summary>
    [CommandFlag(Model.Flag.Standard)]
    [CommandInvocation("help")]
    public class HelpCommand : CommandBase
    {
        #region Fields

        /// <summary>
        /// The command parser.
        /// </summary>
        private readonly ICommandParser commandParser;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpCommand"/> class.
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
        /// <param name="flagService"></param>
        /// <param name="configurationProvider"></param>
        /// <param name="commandParser">
        /// The command Parser.
        /// </param>
        /// <param name="client"></param>
        public HelpCommand(
            string commandSource, 
            IUser user, 
            IEnumerable<string> arguments, 
            ILogger logger,  
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            ICommandParser commandParser,
            IIrcClient client)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
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
                // Yes, this should be a CIE not a ACE, as an ACE will trigger a call to this command
                throw new CommandInvocationException();
            }

            string commandName = this.Arguments.ElementAt(0);
            string key = this.Arguments.Count() > 1 ? this.Arguments.ElementAt(1) : null;

            var command = this.commandParser.GetCommand(
                new CommandMessage { CommandName = commandName }, 
                this.User, 
                this.CommandSource,
                this.Client);

            if (command == null)
            {
                return
                    new CommandResponse
                        {
                            Message = "The specified command could not be found.", 
                            Destination = CommandResponseDestination.PrivateMessage
                        }.ToEnumerable();
            }

            var helpResponses = command.HelpMessage(key).ToList();

            return helpResponses;
        }

        /// <summary>
        /// The help.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
                       {
                           {
                               string.Empty, 
                               new HelpMessage(
                               this.CommandName, 
                               "<Command>", 
                               "Returns all available help for the specified command.")
                           }, 
                           {
                               "command", 
                               new HelpMessage(
                               this.CommandName, 
                               "<Command> <SubCommand>", 
                               "Returns the help for the specified subcommand.")
                           }
                       };
        }

        #endregion
    }
}