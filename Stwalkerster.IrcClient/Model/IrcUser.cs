﻿namespace Stwalkerster.IrcClient.Model
{
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The IRC user.
    /// </summary>
    public class IrcUser : IUser
    {
        /// <summary>
        /// The account.
        /// </summary>
        private string account;

        /// <summary>
        /// Gets or sets a value indicating whether away.
        /// </summary>
        public bool Away { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether skeleton.
        /// </summary>
        public bool Skeleton { get; set; }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public string Account
        {
            get { return this.account; }

            set
            {
                if ((value == "*") || (value == "0"))
                {
                    this.account = null;
                }
                else
                {
                    this.account = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The from prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <returns>
        /// The <see cref="IrcUser" />.
        /// </returns>
        public static IrcUser FromPrefix(string prefix)
        {
            string nick;
            string user = null;
            string host = null;
            if (prefix.Contains("@"))
            {
                var indexOfAt = prefix.IndexOf('@');

                host = prefix.Substring(indexOfAt + 1);
                if (prefix.Contains("!"))
                {
                    var indexOfBang = prefix.IndexOf('!');

                    user = prefix.Substring(indexOfBang + 1, indexOfAt - (indexOfBang + 1));
                    nick = prefix.Substring(0, indexOfBang);
                }
                else
                {
                    nick = prefix.Substring(0, indexOfAt);
                }
            }
            else
            {
                nick = prefix;
            }

            return new IrcUser {Hostname = host, Username = user, Nickname = nick, Skeleton = false};
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((IrcUser) obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Nickname != null ? this.Nickname.GetHashCode() : 0;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Account))
            {
                return string.Format("{0}!{1}@{2}", this.Nickname, this.Username, this.Hostname);
            }

            return string.Format("{0} [{1}!{2}@{3}]", this.Account, this.Nickname, this.Username, this.Hostname);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        protected bool Equals(IrcUser other)
        {
            return string.Equals(this.Nickname, other.Nickname);
        }
    }
}