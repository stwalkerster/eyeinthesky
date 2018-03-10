namespace Stwalkerster.Bot.CommandLib.Services.Interfaces
{
    using Stwalkerster.IrcClient.Events;

    /// <summary>
    /// The i command handler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Called on new messages received by the IRC client
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void OnMessageReceived(object sender, MessageReceivedEventArgs e);
    }
}