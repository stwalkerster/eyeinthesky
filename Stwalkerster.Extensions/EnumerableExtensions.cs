namespace Stwalkerster.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// The implode.
        /// </summary>
        /// <param name="value">
        /// The list.
        /// </param>
        /// <param name="separator">
        /// The separator.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Implode(this IEnumerable<string> value, string separator = " ")
        {
            return string.Join(separator, value.ToArray());
        }
    }
}