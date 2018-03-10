namespace Stwalkerster.Bot.CommandLib.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The CommandParser interface.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="commandMessage">
        /// The command Message.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// The <see cref="ICommand"/>.
        /// </returns>
        ICommand GetCommand(CommandMessage commandMessage, IUser user, string destination, IIrcClient client);

        /// <summary>
        /// The release.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void Release(ICommand command);

        /// <summary>
        /// The parse command message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="nickname">
        /// The nickname of the IRC client.
        /// </param>
        /// <returns>
        /// The <see cref="CommandMessage"/>.
        /// </returns>
        CommandMessage ParseCommandMessage(string message, string nickname);

        /// <summary>
        /// The parse redirection.
        /// </summary>
        /// <param name="inputArguments">
        /// The input arguments.
        /// </param>
        /// <returns>
        /// The <see cref="RedirectionResult"/>.
        /// </returns>
        RedirectionResult ParseRedirection(IEnumerable<string> inputArguments);

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="keyword">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        void RegisterCommand(string keyword, Type implementation);

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="commandName">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <param name="channel">
        /// The channel to limit this registration to
        /// </param>
        void RegisterCommand(string commandName, Type implementation, string channel);
    }
}