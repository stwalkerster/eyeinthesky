namespace Stwalkerster.Bot.CommandLib.Model
{
    /// <summary>
    /// The flag.
    /// </summary>
    public class Flag
    {
        #region Constants

        /// <summary>
        /// Access to access control functionality
        /// </summary>
        public const string Access = "A";

        /// <summary>
        /// Debugging functionality
        /// </summary>
        public const string Debug = "D";

        /// <summary>
        /// The legacy advanced.
        /// </summary>
        public const string Protected = "P";

        /// <summary>
        /// The owner-level access, stuff only the bot owner should be able to use
        /// </summary>
        public const string Owner = "O";

        /// <summary>
        /// The standard uncontroversial commands.
        /// </summary>
        public const string Standard = "B";

        /// <summary>
        /// Configuration access to the bot. This should either be granted per-channel or globally
        /// </summary>
        public const string Configuration = "C";
        
        #endregion
    }
}