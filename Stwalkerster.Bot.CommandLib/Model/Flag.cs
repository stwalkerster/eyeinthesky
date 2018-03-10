// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flag.cs" company="Helpmebot Development Team">
//   Helpmebot is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   Helpmebot is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with Helpmebot.  If not, see http://www.gnu.org/licenses/ .
// </copyright>
// <summary>
//   The flag.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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