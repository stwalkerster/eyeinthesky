namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;

    /// <summary>
    /// The command flag attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandFlagAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandFlagAttribute"/> class.
        /// </summary>
        /// <param name="flag">
        /// The flag.
        /// </param>
        public CommandFlagAttribute(string flag)
        {
            this.Flag = flag;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or flag.
        /// </summary>
        public string Flag { get; private set; }

        #endregion
    }
}