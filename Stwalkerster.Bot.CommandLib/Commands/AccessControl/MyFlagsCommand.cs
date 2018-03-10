namespace Stwalkerster.Bot.CommandLib.Commands.AccessControl
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Extensions;
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The my flags command.
    /// </summary>
    [CommandInvocation("myflags")]
    [CommandInvocation("myaccess")]
    [CommandInvocation("whoami")]
    [CommandFlag(Model.Flag.Standard)]
    public class MyFlagsCommand : CommandBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="MyFlagsCommand"/> class.
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
        /// <param name="client"></param>
        public MyFlagsCommand(
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
            var flagsForUser = this.FlagService.GetFlagsForUser(this.User);

            var message = string.Format(
                "The flags currently available to {0} are: {1}",
                this.User,
                flagsForUser.Implode(string.Empty));

            yield return new CommandResponse { Message = message };
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
                               string.Empty,
                               new HelpMessage(
                               this.CommandName,
                               string.Empty,
                               "Retrieves the flags available to the current user.")
                           }
                       };
        }

        #endregion
    }
}