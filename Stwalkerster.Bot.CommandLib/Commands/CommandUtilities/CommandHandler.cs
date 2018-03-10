namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Messages;

    /// <summary>
    /// The command handler.
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        #region Fields

        /// <summary>
        /// The command parser.
        /// </summary>
        private readonly ICommandParser commandParser;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        private readonly IConfigurationProvider configProvider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandHandler"/> class.
        /// </summary>
        /// <param name="commandParser">
        /// The command parser.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="configProvider"></param>
        public CommandHandler(ICommandParser commandParser, ILogger logger, IConfigurationProvider configProvider)
        {
            this.commandParser = commandParser;
            this.logger = logger;
            this.configProvider = configProvider;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Called on new messages received by the IRC client
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Command != "PRIVMSG")
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(this.ProcessMessageAsync, e);
        }

        /// <summary>

        /// The process message async.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void ProcessMessageAsync(object state)
        {
            var eventArgs = (MessageReceivedEventArgs)state;

            var parameters = eventArgs.Message.Parameters.ToList();
            IIrcClient client = eventArgs.Client;

            string message = parameters[1];

            var commandMessage = this.commandParser.ParseCommandMessage(message, client.Nickname);

            var command = this.commandParser.GetCommand(
                commandMessage,
                client.LookupUser(eventArgs.Message.Prefix),
                parameters[0],
                client);

            if (command == null)
            {
                return;
            }

            try
            {
                IEnumerable<CommandResponse> commandResponses = command.Run();

                foreach (var x in commandResponses)
                {
                    x.RedirectionTarget = command.RedirectionTarget;

                    string destination;

                    switch (x.Destination)
                    {
                        case CommandResponseDestination.ChannelDebug:
                            destination = this.configProvider.DebugChannel;
                            break;
                        case CommandResponseDestination.PrivateMessage:
                            destination = command.User.Nickname;
                            break;
                        case CommandResponseDestination.Default:
                            if (command.CommandSource == client.Nickname)
                            {
                                // PMs to the bot.
                                destination = command.User.Nickname;
                            }
                            else
                            {
                                destination = command.CommandSource;
                            }
                            break;
                        default:
                            destination = command.CommandSource;
                            this.logger.Warn("Command response has an unknown destination!");
                            break;
                    }

                           
                    if (x.Type == CommandResponseType.Notice)
                    {
                        client.Send(new Notice(destination, x.CompileMessage()));
                    }
                    else
                    {
                        client.Send(new PrivateMessage(destination, x.CompileMessage()));
                    }
                }
            }
            finally
            {
                // wait 30 seconds for the post command events to finish execution, before finally killing the command
                command.CommandCompletedSemaphore.WaitOne(30000);
                this.commandParser.Release(command);
            }
        }

        #endregion
    }
}