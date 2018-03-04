namespace Stwalkerster.IrcClient.Model
{
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The server user.
    /// </summary>
    public class ServerUser : IUser
    {
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        public string
            Nickname
        {
            get { return string.Empty; }

            set { }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get { return string.Empty; }

            set { }
        }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public string Hostname
        {
            get { return string.Empty; }

            set { }
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public string Account
        {
            get { return string.Empty; }

            set { }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return "[SERVER]";
        }
    }
}