namespace Stwalkerster.Bot.CommandLib.Commands.BotManagement
{
    using System;
    using System.Collections.Generic;
    
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The die command.
    /// </summary>
    [CommandInvocation("die")]
    [CommandFlag(Model.Flag.Owner)]
    public class DieCommand : CommandBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="DieCommand"/> class.
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
        /// <param name="client"></param>
        /// <param name="flagService"></param>
        /// <param name="configurationProvider"></param>
        public DieCommand(
            string commandSource, 
            IUser user, 
            IEnumerable<string> arguments, 
            ILogger logger, 
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
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
            Environment.Exit(0);
            
            return new CommandResponse[0];
        }

        /// <summary>
        /// The help.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary{String, HelpMessage}"/>.
        /// </returns>
        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
                       {
                           {
                               "die", 
                               new HelpMessage(
                               this.CommandName, 
                               string.Empty, 
                               "Shuts down the bot.")
                           }
                       };
        }

        #endregion
    }
}