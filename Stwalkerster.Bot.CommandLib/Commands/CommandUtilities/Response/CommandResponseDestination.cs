namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response
{
    /// <summary>
    /// Command response destinations
    /// </summary>
    public enum CommandResponseDestination
    {
        /// <summary>
        /// Back to the channel from whence it came
        /// </summary>
        Default,

        /// <summary>
        /// To the debugging channel
        /// </summary>
        ChannelDebug,

        /// <summary>
        /// Back to the user in a private message
        /// </summary>
        PrivateMessage
    }
}