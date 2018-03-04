#region Usings

using System;

#endregion

namespace EyeInTheSky
{
    /// <summary>
    /// Class holding globally accessible functions
    /// </summary>
    [Obsolete]
    public class GlobalFunctions
    {
        /// <summary>
        ///   Remove the first item from an array, and return the item
        /// </summary>
        /// <param name = "list">The array in question</param>
        /// <returns>The first item from the array</returns>
        [Obsolete]
        public static string popFromFront(ref string[] list)
        {
            string firstItem = list[0];
            list = string.Join(" ", list, 1, list.Length - 1).Split(' ');
            return firstItem;
        }

        /// <summary>
        ///   Log an exception to the log and IRC
        /// </summary>
        /// <param name = "ex">The exception thrown</param>
        [Obsolete]
        public static void errorLog(Exception ex)
        {
            Logger.instance().addToLog(ex + ex.StackTrace, Logger.LogTypes.Error);
        }
    }
}