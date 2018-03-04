namespace Stwalkerster.IrcClient.Interfaces
{
    /// <summary>
    ///     The IRC Configuration interface.
    /// </summary>
    public interface IIrcConfiguration
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether to authenticate to services.
        /// </summary>

        bool AuthToServices { get; }

        /// <summary>
        ///     Gets the hostname.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        ///     Gets the nickname.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        ///     Gets the hostname.
        /// </summary>
        int Port { get; }

        /// <summary>
        ///     Gets the real name.
        /// </summary>
        string RealName { get; }

        /// <summary>
        ///     Gets the username.
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     Gets the username.
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     Gets a value indicating whether to connect with SSL.
        /// </summary>
        bool Ssl { get; }
        
        string ClientName { get; }
        
        #endregion
    }
}