namespace Stwalkerster.IrcClient.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    /// The CTCP extensions.
    /// </summary>
    public static class CtcpExtensions
    {
        /// <summary>
        /// Wrap a message as a CTCP command
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ctcpCommand">
        /// The CTCP command.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SetupForCtcp(this string message, string ctcpCommand)
        {
            var asc = new ASCIIEncoding();
            byte[] ctcp = {Convert.ToByte(1)};
            return asc.GetString(ctcp) + ctcpCommand.ToUpper()
                                       + (message == string.Empty ? string.Empty : " " + message) + asc.GetString(ctcp);
        }
    }
}