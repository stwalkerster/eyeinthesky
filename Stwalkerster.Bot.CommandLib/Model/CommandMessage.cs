namespace Stwalkerster.Bot.CommandLib.Model
{
    /// <summary>
    /// The command message.
    /// </summary>
    public class CommandMessage
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the argument list.
        /// </summary>
        public string ArgumentList { get; set; }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether override silence.
        /// </summary>
        public bool OverrideSilence { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param>
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

            return this.Equals((CommandMessage)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ArgumentList != null ? this.ArgumentList.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.CommandName != null ? this.CommandName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.OverrideSilence.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "ArgumentList: {0}, CommandName: {1}, OverrideSilence: {2}", 
                this.ArgumentList, 
                this.CommandName, 
                this.OverrideSilence);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool Equals(CommandMessage other)
        {
            return string.Equals(this.ArgumentList, other.ArgumentList)
                   && string.Equals(this.CommandName, other.CommandName)
                   && this.OverrideSilence.Equals(other.OverrideSilence);
        }

        #endregion
    }
}