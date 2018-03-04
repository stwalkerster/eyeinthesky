namespace Stwalkerster.IrcClient.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// The IRC channel.
    /// </summary>
    public class IrcChannel
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The users.
        /// </summary>
        private readonly Dictionary<string, IrcChannelUser> users;

        /// <summary>
        /// Initialises a new instance of the <see cref="IrcChannel" /> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public IrcChannel(string name)
        {
            this.name = name;
            this.users = new Dictionary<string, IrcChannelUser>();
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        public Dictionary<string, IrcChannelUser> Users
        {
            get { return this.users; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
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

            return this.Equals((IrcChannel) obj);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Name != null ? this.Name.GetHashCode() : 0;
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
        protected bool Equals(IrcChannel other)
        {
            return string.Equals(this.Name, other.Name);
        }
    }
}