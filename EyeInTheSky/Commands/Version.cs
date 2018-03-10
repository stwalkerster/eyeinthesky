namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Reflection;
    using Stwalkerster.IrcClient;
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Version : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void Execute(IUser source, string destination, IEnumerable<string> tokens)
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var ircVersion = Assembly.GetAssembly(typeof(IrcClient)).GetName().Version;

            this.Client.SendMessage(
                destination,
                "EyeInTheSky v" + assemblyVersion + ", using Stwalkerster.IrcClient v" + ircVersion);
        }

        #endregion
    }
}
