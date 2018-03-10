namespace Stwalkerster.Bot.CommandLib.Attributes
{
    using System;

    /// <summary>
    /// The command invocation attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CommandInvocationAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandInvocationAttribute"/> class.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        public CommandInvocationAttribute(string commandName)
        {
            this.CommandName = commandName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string CommandName { get; private set; }

        #endregion
    }
}