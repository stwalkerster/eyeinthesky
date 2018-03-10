namespace Stwalkerster.Bot.CommandLib.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.ExtensionMethods;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.TypedFactories;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The command parser.
    /// </summary>
    public class CommandParser : ICommandParser
    {
        #region Fields

        private readonly IConfigurationProvider configProvider;

        /// <summary>
        /// The command factory.
        /// </summary>
        private readonly ICommandTypedFactory commandFactory;

        /// <summary>
        /// The command trigger.
        /// </summary>
        private readonly string commandTrigger;

        /// <summary>
        /// The commands.
        /// </summary>
        private readonly Dictionary<string, Dictionary<CommandRegistration, Type>> commands;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandParser"/> class.
        /// </summary>
        /// <param name="configProvider">
        /// The configuration provider.
        /// </param>
        /// <param name="commandFactory">
        /// The command Factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CommandParser(
            IConfigurationProvider configProvider,
            ICommandTypedFactory commandFactory,
            ILogger logger)
        {
            this.commandTrigger = configProvider.CommandPrefix;
            this.configProvider = configProvider;
            this.commandFactory = commandFactory;
            this.logger = logger;
            var types = Assembly.GetExecutingAssembly().GetTypes();

            this.commands = new Dictionary<string, Dictionary<CommandRegistration, Type>>();
            foreach (var type in types)
            {
                if (!type.IsSubclassOf(typeof(CommandBase)))
                {
                    // Not a new command class;
                    continue;
                }

                var customAttributes = type.GetCustomAttributes(typeof(CommandInvocationAttribute), false);
                if (customAttributes.Length > 0)
                {
                    foreach (var attribute in customAttributes)
                    {
                        var commandName = ((CommandInvocationAttribute)attribute).CommandName;

                        if (commandName != string.Empty)
                        {
                            this.RegisterCommand(commandName, type);
                        }
                    }
                }
            }

            this.logger.InfoFormat("Initialised Command Parser with {0} commands.", this.commands.Count);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="commandMessage">
        /// The command Message.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <returns>
        /// The <see cref="ICommand"/>.
        /// </returns>
        public ICommand GetCommand(CommandMessage commandMessage, IUser user, string destination, IIrcClient client)
        {
            if (commandMessage == null || commandMessage.CommandName == null)
            {
                this.logger.Debug("Returning early from GetCommand - null message!");
                return null;
            }

            IEnumerable<string> originalArguments = new List<string>();

            if (commandMessage.ArgumentList != null)
            {
                originalArguments =
                    commandMessage.ArgumentList.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            var redirectionResult = this.ParseRedirection(originalArguments);
            IEnumerable<string> arguments = redirectionResult.Arguments.ToList();

            var commandName = commandMessage.CommandName.ToLower(CultureInfo.InvariantCulture);
            var commandType = this.GetRegisteredCommand(commandName, destination);

            if (commandType != null)
            {   
                this.logger.InfoFormat("Creating command object of type {0}", commandType);

                try
                {
                    var command = this.commandFactory.CreateType(commandType, destination, user, arguments);

                    command.RedirectionTarget = redirectionResult.Target;
                    command.OriginalArguments = originalArguments;

                    return command;
                }
                catch (TargetInvocationException e)
                {
                    this.logger.Error("Unable to create instance of command.", e.InnerException);
                    client.SendMessage(this.configProvider.DebugChannel, e.InnerException.Message.Replace("\r\n", " "));
                }
            }

            return null;
        }

        /// <summary>
        /// The parse redirection.
        /// </summary>
        /// <param name="inputArguments">
        /// The input arguments.
        /// </param>
        /// <returns>
        /// The <see cref="RedirectionResult"/>.
        /// </returns>
        public RedirectionResult ParseRedirection(IEnumerable<string> inputArguments)
        {
            var targetList = new List<string>();
            var channelList = new List<string>();
            var parsedArguments = new List<string>();

            var redirecting = false;

            foreach (var argument in inputArguments)
            {
                if (redirecting)
                {
                    redirecting = false;
                    if (argument.StartsWith("#"))
                    {
                        channelList.Add(argument);
                    }
                    else
                    {
                        targetList.Add(argument);
                    }
                    
                    continue;
                }

                if (argument == ">")
                {
                    redirecting = true;
                    continue;
                }

                if (argument.StartsWith(">"))
                {
                    var arg = argument.Substring(1);

                    if (arg.StartsWith("#"))
                    {
                        channelList.Add(arg);
                    }
                    else
                    {
                        targetList.Add(arg);
                    }

                    continue;
                }

                parsedArguments.Add(argument);
            }

            // last word on line was >
            if (redirecting)
            {
                parsedArguments.Add(">");
            }

            return new RedirectionResult(parsedArguments, targetList, channelList);
        }

        /// <summary>
        /// The parse command message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="nickname">
        /// The nickname.
        /// </param>
        /// <returns>
        /// The <see cref="CommandMessage"/>.
        /// </returns>
        public CommandMessage ParseCommandMessage(string message, string nickname)
        {
            var validCommand =
                new Regex(
                    @"^(?:" + this.commandTrigger + @"(?:(?<botname>(?:" + nickname + @")|(?:"
                    + nickname.ToLower() + @")) )?(?<cmd>[" + "0-9a-z-_" + "]+)|(?<botname>(?:" + nickname
                    + @")|(?:" + nickname.ToLower() + @"))[ ,>:](?: )?(?<cmd>[" + "0-9a-z-_"
                    + "]+))(?: )?(?<args>.*?)(?:\r)?$");

            Match m = validCommand.Match(message);

            if (m.Length > 0)
            {
                var commandMessage = new CommandMessage();

                if (m.Groups["botname"].Length > 0)
                {
                    commandMessage.OverrideSilence = true;
                }

                if (m.Groups["cmd"].Length > 0)
                {
                    commandMessage.CommandName = m.Groups["cmd"].Value.Trim();
                }
                else
                {
                    return null;
                }

                if (m.Groups["args"].Length > 0)
                {
                    commandMessage.ArgumentList = m.Groups["args"].Length > 0
                                                      ? m.Groups["args"].Value.Trim()
                                                      : string.Empty;
                }
                else
                {
                    commandMessage.ArgumentList = string.Empty;
                }

                return commandMessage;
            }

            return null;
        }

        /// <summary>
        /// The release.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void Release(ICommand command)
        {
            this.commandFactory.Release(command);
        }

        #endregion

        #region Command Registration

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="commandName">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        public void RegisterCommand(string commandName, Type implementation)
        {
            this.RegisterCommand(commandName, implementation, null);
        }

        /// <summary>
        /// The register command.
        /// </summary>
        /// <param name="commandName">
        /// The keyword.
        /// </param>
        /// <param name="implementation">
        /// The implementation.
        /// </param>
        /// <param name="channel">
        /// The channel to limit this registration to
        /// </param>
        public void RegisterCommand(string commandName, Type implementation, string channel)
        {
            if (!this.commands.ContainsKey(commandName))
            {
                this.commands.Add(commandName, new Dictionary<CommandRegistration, Type>());
            }

            this.commands[commandName].Add(new CommandRegistration(channel, implementation), implementation);
        }
       
        #endregion

        #region Private Methods

        private Type GetRegisteredCommand(string commandName, string destination)
        {
            Dictionary<CommandRegistration, Type> commandRegistrationSet;
            if (!this.commands.TryGetValue(commandName, out commandRegistrationSet))
            {
                // command doesn't exist anywhere
                return null;
            }

            var channelRegistration = commandRegistrationSet.Keys.FirstOrDefault(x => x.Channel == destination);

            if (channelRegistration != null)
            {
                // This command is defined locally in this channel
                return commandRegistrationSet[channelRegistration];
            }

            var globalRegistration = commandRegistrationSet.Keys.FirstOrDefault(x => x.Channel == null);

            if (globalRegistration != null)
            {
                // This command is not defined locally, but is defined globally
                return commandRegistrationSet[globalRegistration];
            }

            // This command has a registration entry, but isn't defined in this channel or globally.
            return null;
        }

        #endregion

    }
}