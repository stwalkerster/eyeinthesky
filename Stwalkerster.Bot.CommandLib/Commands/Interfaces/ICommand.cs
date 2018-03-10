namespace Stwalkerster.Bot.CommandLib.Commands.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The Command interface.
    /// </summary>
    public interface ICommand
    {
        #region Public Properties

        /// <summary>
        /// Gets the arguments to the command.
        /// </summary>
        IEnumerable<string> Arguments { get; }

        /// <summary>
        /// Gets the source (where the command was triggered).
        /// </summary>
        string CommandSource { get; }

        /// <summary>
        /// Gets the flag required to execute.
        /// </summary>
        string Flag { get; }

        /// <summary>
        /// Gets or sets the original arguments.
        /// </summary>
        IEnumerable<string> OriginalArguments { get; set; }

        /// <summary>
        /// Gets or sets the redirection target.
        /// </summary>
        IEnumerable<string> RedirectionTarget { get; set; }

        /// <summary>
        /// Gets the user who triggered the command.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// The commandCompletedSemaphore.
        /// </summary>
        Semaphore CommandCompletedSemaphore { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns true if the command can be executed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool CanExecute();

        /// <summary>
        /// The help message.
        /// </summary>
        /// <param name="helpKey">
        /// The help Key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        IEnumerable<CommandResponse> HelpMessage(string helpKey = null);

        /// <summary>
        /// The run.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        IEnumerable<CommandResponse> Run();

        #endregion
    }
}