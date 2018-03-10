namespace Stwalkerster.Extensions
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// The to base 64.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToBase64(this string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// The to stream.
        /// </summary>
        /// <param name="data">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public static Stream ToStream(this string data)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}