using System;

namespace EyeInTheSky
{
    using Castle.Core.Logging;
    using Microsoft.Practices.ServiceLocation;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    abstract class GenericCommand
    {
        protected ILogger Logger { get; private set; }
        protected StalkConfiguration StalkConfig { get; private set; }
        protected IIrcClient Client { get; private set; }

        public static GenericCommand Create(string command, ILogger logger, StalkConfiguration stalkConfig, IIrcClient client)
        {
            command = command.Substring(1);
            string trueCommand = command.Substring(0, 1).ToUpper() + command.Substring(1).ToLower();
            trueCommand = "EyeInTheSky.Commands." + trueCommand;

            Type t = Type.GetType(trueCommand);
            if (t == null)
            {
                return null;
            }

            var commandInstance = (GenericCommand) Activator.CreateInstance(t);
            commandInstance.Logger = logger;
            commandInstance.StalkConfig = stalkConfig;
            commandInstance.Client = client;
            
            return commandInstance;
        }

        public void Run(IUser source, string destination, string[] tokens)
        {
            var configuration = ServiceLocator.Current.GetInstance<AppConfiguration>();
            if (destination != configuration.FreenodeChannel)
            {
                this.Client.SendNotice(source.Nickname, "Access denied.");
                return;
            }

            this.Execute(source, destination, tokens);
        }

        protected abstract void Execute(IUser source, string destination, string[] tokens);
    }
}