namespace Stwalkerster.Bot.CommandLib.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The typed factory extensions.
    /// </summary>
    public static class TypedFactoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandSource">
        /// The command Source.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static ICommand CreateType(
            this ICommandTypedFactory factory,
            Type commandType,
            string commandSource,
            IUser user,
            IEnumerable<string> arguments)
        {
            return
                (ICommand)
                typeof(ICommandTypedFactory).GetMethod("Create")
                    .MakeGenericMethod(commandType)
                    .Invoke(factory, new object[] { commandSource, user, arguments });
        }

        #endregion
    }
}