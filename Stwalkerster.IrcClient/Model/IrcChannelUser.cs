namespace Stwalkerster.IrcClient.Model
{
    /// <summary>
    /// The channel user.
    /// </summary>
    public class IrcChannelUser
    {
        /// <summary>
        /// The channel.
        /// </summary>
        private readonly string channel;

        /// <summary>
        /// The user.
        /// </summary>
        private readonly IrcUser user;

        /// <summary>
        /// Initialises a new instance of the <see cref="IrcChannelUser" /> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        public IrcChannelUser(IrcUser user, string channel)
        {
            this.user = user;
            this.channel = channel;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public IrcUser User
        {
            get { return this.user; }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        public string Channel
        {
            get { return this.channel; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether operator.
        /// </summary>
        public bool Operator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether voice.
        /// </summary>
        public bool Voice { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "[{0} {1}{2} {3}]",
                this.Channel,
                this.Operator ? "@" : string.Empty,
                this.Voice ? "+" : string.Empty,
                this.User);
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

            return this.Equals((IrcChannelUser) obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.User != null ? this.User.GetHashCode() : 0) * 397)
                       ^ (this.Channel != null ? this.Channel.GetHashCode() : 0);
            }
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
        protected bool Equals(IrcChannelUser other)
        {
            return Equals(this.User, other.User) && string.Equals(this.Channel, other.Channel);
        }
    }
}