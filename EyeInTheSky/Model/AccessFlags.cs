namespace EyeInTheSky.Model
{
    using Stwalkerster.Bot.CommandLib.Model;

    public class AccessFlags : Flag
    {
        /// <summary>
        /// Configuration access to the bot. This should either be granted per-channel or globally
        /// </summary>
        public const string Configuration = "C";
        public const string Admin = "A";
    }
}