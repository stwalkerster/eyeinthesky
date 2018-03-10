namespace Stwalkerster.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// The object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// The to list.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <typeparam name="T">
        /// The type of object
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            return new List<T> {obj};
        }
    }
}