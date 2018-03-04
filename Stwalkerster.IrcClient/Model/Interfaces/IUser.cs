namespace Stwalkerster.IrcClient.Model.Interfaces
{
    /// <summary>
    /// The User interface.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>The nickname.</value>
        string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>The hostname.</value>
        string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        string Account { get; set; }
    }
}