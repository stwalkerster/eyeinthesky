namespace Stwalkerster.IrcClient.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// The list extensions.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// The pop from front.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <typeparam name="T">
        /// the type of list
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static T PopFromFront<T>(this List<T> list)
        {
            var foo = list[0];
            list.RemoveAt(0);

            return foo;
        }
    }
}