namespace Stwalkerster.Bot.CommandLib.Commands.CommandUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Castle.Core.Internal;
    using Castle.Core.Logging;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Commands.Interfaces;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services;
    using Stwalkerster.Extensions;
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    /// <summary>
    /// The command base.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        #region Fields

        private readonly IConfigurationProvider configurationProvider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        /// <param name="commandSource">
        /// The command source.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="flagService"></param>
        /// <param name="configurationProvider"></param>
        /// <param name="client"></param>
        protected CommandBase(
            string commandSource, 
            IUser user, 
            IEnumerable<string> arguments, 
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client)
        {
            this.FlagService = flagService;
            this.configurationProvider = configurationProvider;
            this.CommandCompletedSemaphore = new Semaphore(1, 1);
            this.Logger = logger;
            this.Client = client;
            this.CommandSource = commandSource;
            this.User = user;
            this.Arguments = arguments;
        }

        #endregion

        #region Public Properties

        public bool Executed { get; private set; }

        /// <summary>
        /// Gets the command Completed Semaphore.
        /// <para>
        /// This semaphore can be set with <code>CommandCompletedSemaphore.WaitOne()</code> to suspend destruction of the command for a maximum of thirty seconds.
        /// </para>
        /// </summary>
        public Semaphore CommandCompletedSemaphore { get; private set; }

        /// <summary>
        /// Gets the arguments to the command.
        /// </summary>
        public IEnumerable<string> Arguments { get; private set; }
        
        public IFlagService FlagService { get; private set; }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string CommandName
        {
            get
            {
                var customAttributes = this.GetType().GetCustomAttributes(typeof(CommandInvocationAttribute), false);
                if (customAttributes.Length > 0)
                {
                    return ((CommandInvocationAttribute)customAttributes.First()).CommandName;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the source (where the command was triggered).
        /// </summary>
        public string CommandSource { get; private set; }

        /// <summary>
        /// Gets the flag required to execute.
        /// </summary>
        public string Flag
        {
            get
            {
                return this.GetType().GetAttribute<CommandFlagAttribute>().Flag;
            }
        }

        /// <summary>
        /// Gets or sets the original arguments.
        /// </summary>
        public IEnumerable<string> OriginalArguments { get; set; }

        /// <summary>
        /// Gets or sets the redirection target.
        /// </summary>
        public IEnumerable<string> RedirectionTarget { get; set; }

        /// <summary>
        /// Gets the user who triggered the command.
        /// </summary>
        public IUser User { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        protected IIrcClient Client { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns true if the command can be executed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool CanExecute()
        {
            return this.FlagService.UserHasFlag(this.User, this.Flag);
        }

        /// <summary>
        /// The help message.
        /// </summary>
        /// <param name="helpKey">
        /// The help Key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        public IEnumerable<CommandResponse> HelpMessage(string helpKey = null)
        {
            var helpMessages = this.Help();

            var commandTrigger = this.configurationProvider.CommandTrigger;

            if (helpMessages == null)
            {
                return new List<CommandResponse>();
            }

            if (helpKey != null && helpMessages.ContainsKey(helpKey))
            {
                return helpMessages[helpKey].ToCommandResponses(commandTrigger);
            }

            var help = new List<CommandResponse>();
            foreach (var helpMessage in helpMessages)
            {
                help.AddRange(helpMessage.Value.ToCommandResponses(commandTrigger));
            }

            return help;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        public IEnumerable<CommandResponse> Run()
        {
            if (this.Executed)
            {
                throw new Exception("Already executed instance of command");
            }

            try
            {
                if (this.CanExecute())
                {
                    try
                    {
                        var commandResponses = this.Execute() ?? new List<CommandResponse>();
                        var completedResponses = this.OnCompleted() ?? new List<CommandResponse>();

                        // Resolve the list into a concrete list before committing the transaction.
                        var responses = commandResponses.Concat(completedResponses).ToList();

                        return responses;
                    }
                    catch (CommandInvocationException e)
                    {
                        this.Logger.Info("Command encountered an issue from invocation.");

                        return this.HelpMessage(e.HelpKey);
                    }
                    catch (ArgumentCountException e)
                    {
                        this.Logger.Info("Command executed with missing arguments.");

                        var responses = new List<CommandResponse>
                                            {
                                                new CommandResponse
                                                    {
                                                        Destination =
                                                            CommandResponseDestination
                                                            .Default,
                                                        Message = e.Message
                                                    }
                                            };

                        responses.AddRange(this.HelpMessage(e.HelpKey));

                        return responses;
                    }
                    catch (CommandExecutionException e)
                    {
                        this.Logger.Warn("Command encountered an issue during execution.", e);

                        return new List<CommandResponse>
                                   {
                                       new CommandResponse
                                           {
                                               Destination =
                                                   CommandResponseDestination
                                                   .Default,
                                               Message = e.Message
                                           }
                                   };
                    }
                }

                this.Logger.InfoFormat("Access denied for user {0}", this.User);

                IEnumerable<CommandResponse> accessDeniedResponses = this.OnAccessDenied()
                                                                     ?? new List<CommandResponse>();

                return accessDeniedResponses;
            }
            finally
            {
                this.Executed = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        protected abstract IEnumerable<CommandResponse> Execute();

        /// <summary>
        /// The help.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary{String, HelpMessage}"/>.
        /// </returns>
        protected virtual IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
                       {
                           {
                               string.Empty, 
                               new HelpMessage(
                               this.CommandName, 
                               string.Empty, 
                               "No help is available for this command.")
                           }
                       };
        }

        /// <summary>
        /// The on access denied.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        protected virtual IEnumerable<CommandResponse> OnAccessDenied()
        {
            var response = new CommandResponse
                               {
                                   Destination = CommandResponseDestination.PrivateMessage, 
                                   Message = "Access denied, sorry. :(",
                                   IgnoreRedirection = true
                               };

            return response.ToEnumerable();
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{CommandResponse}"/>.
        /// </returns>
        protected virtual IEnumerable<CommandResponse> OnCompleted()
        {
            return null;
        }

        #endregion
    }
}