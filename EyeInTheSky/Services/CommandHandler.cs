namespace EyeInTheSky.Services
{
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.Extensions;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class CommandHandler
    {
        private readonly IAppConfiguration configuration;
        private readonly ILogger logger;
        private readonly StalkConfiguration stalkConfig;

        public CommandHandler(IAppConfiguration configuration, ILogger logger, StalkConfiguration stalkConfig)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.stalkConfig = stalkConfig;
        }

        public void OnReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Command != "PRIVMSG")
            {
                return;
            }

            var parameters = e.Message.Parameters.ToList();
            string destination = parameters[0];
            string message = parameters[1];
            var source = IrcUser.FromPrefix(e.Message.Prefix);
            
            if (message.Substring(0, 1) != this.configuration.CommandPrefix)
            {
                return;
            }

            this.logger.DebugFormat("Received command message: {0}", message);
            
            var tokens = message.Split(' ').ToList();
            var command = tokens.PopFromFront();

            var ircClient = (IIrcClient)sender;
            
            GenericCommand cmd = GenericCommand.Create(command, this.logger.CreateChildLogger(command), this.stalkConfig, ircClient);
            if (cmd != null)
            {
                cmd.Run(IrcUser.FromPrefix(e.Message.Prefix), destination, tokens.ToArray());
            }
            else
            {
                ircClient.SendNotice(source.Nickname, "Command not found.");
            }
        }
    }
}