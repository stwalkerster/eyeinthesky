namespace Stwalkerster.Bot.CommandLib.TypedFactories
{
    using System.Collections.Generic;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The CommandTypedFactory interface.
    /// </summary>
    public interface ICommandTypedFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="commandSource">
        /// The command Source.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="T">
        /// The command type
        /// </typeparam>
        /// <returns>
        /// The <see cref="ICommand"/>.
        /// </returns>
        T Create<T>(string commandSource, IUser user, IEnumerable<string> arguments)
            where T : ICommand;

        /// <summary>
        /// The release.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void Release(ICommand command);

        #endregion
    }
}