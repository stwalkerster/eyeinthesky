namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using MimeKit;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("account")]
    [CommandFlag(CLFlag.Standard)]
    public class AccountCommand : CommandBase
    {
        private readonly IAppConfiguration appConfig;
        private readonly INotificationTemplates templates;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IEmailHelper emailHelper;

        public AccountCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IBotUserConfiguration botUserConfiguration,
            IEmailHelper emailHelper,
            IAppConfiguration config,
            INotificationTemplates templates) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.appConfig = config;
            this.templates = templates;
            this.botUserConfiguration = botUserConfiguration;
            this.emailHelper = emailHelper;
        }

        public override bool CanExecute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                return base.CanExecute();
            }

            var mode = tokenList.PopFromFront();

            switch (mode)
            {
                case "forcedelete":
                case "forcenomail":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.Admin, null);
                case "register":
                case "delete":
                case "deleteconfirm":
                case "email":
                case "verify":
                    return base.CanExecute();
                default:
                    return false;
            }
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(1, this.Arguments.Count());
            }

            string mode = tokenList.PopFromFront();

            switch (mode)
            {
                case "register":
                    return this.RegisterMode();
                case "delete":
                    return this.DeleteMode();
            }

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count(), mode);
            }

            switch (mode)
            {
                case "forcedelete":
                    return this.ForceDeleteMode(tokenList);
                case "forcenomail":
                    return this.ForceNoMailMode(tokenList);
                case "deleteconfirm":
                    return this.DeleteConfirmMode(tokenList);
                case "email":
                    return this.EmailMode(tokenList);
                case "verify":
                    return this.VerifyMode(tokenList);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> VerifyMode(List<string> tokenList)
        {
            IBotUser botUser;
            var resp = this.GetBotUser(out botUser);
            if (resp != null)
            {
                return resp;
            }

            var token = tokenList.FirstOrDefault();

            var result = botUser.ConfirmEmailAddress(token);
            this.botUserConfiguration.Save();

            switch (result)
            {
                case "ok":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "Your email address has been verified.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };

                case "token-mismatch":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "The token provided does not match the expected token.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };

                case "token-expired":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message =
                                "This token has expired. Please re-set your email address to restart the verification process.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };

                case "no-token":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "There is no token on record for this action. Perhaps it expired?",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };

                default:
                    throw new CommandErrorException("Unknown response from account verification validity check");
            }
        }

        private IEnumerable<CommandResponse> EmailMode(List<string> tokenList)
        {
            IBotUser botUser;
            var resp = this.GetBotUser(out botUser);
            if (resp != null)
            {
                return resp;
            }

            var address = tokenList.FirstOrDefault();
            if (address == "none")
            {
                botUser.EmailAddress = null;
                this.botUserConfiguration.Save();
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Your email address has been removed.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            bool valid = false;
            try
            {
                MailboxAddress.Parse(address);
                valid = true;
            }
            catch (ParseException)
            {
            }

            if (!valid)
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "The email address you provided is invalid.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            botUser.EmailAddress = address;
            this.botUserConfiguration.Save();

            var body = string.Format(
                this.templates.EmailAccountConfirmationBody,
                this.Client.Nickname,
                botUser.EmailConfirmationToken,
                this.appConfig.CommandPrefix,
                botUser.EmailConfirmationTimestamp.HasValue
                    ? botUser.EmailConfirmationTimestamp.Value.ToString(this.appConfig.DateFormat)
                    : "never"
            );

            this.emailHelper.SendEmail(
                body,
                this.templates.EmailAccountConfirmationSubject,
                null,
                botUser.EmailAddress);

            return new[]
            {
                new CommandResponse
                {
                    Message = "Please check your email for further instructions.",
                    Destination = CommandResponseDestination.PrivateMessage
                }
            };
        }

        private IEnumerable<CommandResponse> DeleteConfirmMode(List<string> tokenList)
        {
            IBotUser botUser;
            var resp = this.GetBotUser(out botUser);
            if (resp != null)
            {
                return resp;
            }

            var result = botUser.ValidateDeleteAccount(tokenList.FirstOrDefault());
            switch (result)
            {
                case "ok":
                    this.botUserConfiguration.Remove(botUser.Identifier);
                    this.botUserConfiguration.Save();

                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "Your account has been deleted.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };
                case "token-mismatch":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "The token provided does not match the expected token.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };
                case "token-expired":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message =
                                "This token has expired. Please restart the deletion process if you wish to proceed.",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };
                case "no-token":
                    return new[]
                    {
                        new CommandResponse
                        {
                            Message = "There is no token on record for this action. Perhaps it expired?",
                            Destination = CommandResponseDestination.PrivateMessage
                        }
                    };
                default:
                    throw new CommandErrorException("Unknown response from account deletion validity check");
            }
        }

        private IEnumerable<CommandResponse> ForceDeleteMode(List<string> tokenList)
        {
            var accountKey = string.Format("$a:{0}", tokenList.FirstOrDefault());
            var botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                yield return new CommandResponse
                {
                    Message = "No such user",
                    Destination = CommandResponseDestination.PrivateMessage
                };
                yield break;
            }
            
            this.botUserConfiguration.Remove(accountKey);
            this.botUserConfiguration.Save();
            
            yield return new CommandResponse
            {
                Message = "Deleted user " + accountKey
            };
        }

        private IEnumerable<CommandResponse> ForceNoMailMode(List<string> tokenList)
        {
            var accountKey = string.Format("$a:{0}", tokenList.FirstOrDefault());
            var botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                yield return new CommandResponse
                {
                    Message = "No such user",
                    Destination = CommandResponseDestination.PrivateMessage
                };
                yield break;
            }

            botUser.EmailAddress = null;
            
            this.botUserConfiguration.Save();
            
            yield return new CommandResponse
            {
                Message = "Deleted email address for " + accountKey
            };        
        }

        private IEnumerable<CommandResponse> DeleteMode()
        {
            IBotUser botUser;
            var resp = this.GetBotUser(out botUser);
            if (resp != null)
            {
                return resp;
            }

            botUser.GarbageCollectTokens();
            botUser.PrepareDeleteAccount();
            this.botUserConfiguration.Save();

            if (botUser.EmailAddressConfirmed && this.appConfig.EmailConfiguration != null)
            {
                var body = string.Format(
                    this.templates.EmailAccountDeletionBody,
                    this.Client.Nickname,
                    botUser.DeletionConfirmationToken,
                    this.appConfig.CommandPrefix,
                    botUser.DeletionConfirmationTimestamp.HasValue
                        ? botUser.DeletionConfirmationTimestamp.Value.ToString(this.appConfig.DateFormat)
                        : "never"
                );

                this.emailHelper.SendEmail(
                    body,
                    this.templates.EmailAccountDeletionSubject,
                    null,
                    botUser.EmailAddress);

                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Please check your email for further instructions.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            // do it via IRC
            return new[]
            {
                new CommandResponse
                {
                    Message =
                        "Are you absolutely sure you wish to delete your account? To proceed, please run the following command:",
                    Destination = CommandResponseDestination.PrivateMessage
                },

                new CommandResponse
                {
                    Message = string.Format(
                        "  {1}account deleteconfirm {0}",
                        botUser.DeletionConfirmationToken,
                        this.appConfig.CommandPrefix),
                    Destination = CommandResponseDestination.PrivateMessage
                }
            };
        }

        private IEnumerable<CommandResponse> RegisterMode()
        {
            if (this.User.Account == null)
            {
                yield return new CommandResponse
                {
                    Message = "Please identify to NickServ first!",
                    Destination = CommandResponseDestination.PrivateMessage
                };
                yield break;
            }

            var mask = new IrcUserMask(string.Format("$a:{0}", this.User.Account), this.Client);
            var user = new BotUser(mask);
            this.botUserConfiguration.Add(user);
            this.botUserConfiguration.Save();

            yield return new CommandResponse
            {
                Message = "Registered using NickServ account " + this.User.Account,
            };
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            var help = new Dictionary<string, HelpMessage>
            {
                {
                    "register",
                    new HelpMessage(
                        this.CommandName,
                        "register",
                        "Registers your NickServ account with the bot")
                },
                {
                    "delete",
                    new HelpMessage(
                        this.CommandName,
                        "delete",
                        "Deletes your account with the bot")
                },
                {
                    "email",
                    new HelpMessage(
                        this.CommandName,
                        new[] {"email <address>", "email none"},
                        "Sets or removes your email address")
                },
            };

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.Admin, null))
            {
                help.Add(
                    "forcedelete",
                    new HelpMessage(
                        this.CommandName,
                        "forcedelete <account>",
                        "Forcibly deletes a user's account"));

                help.Add(
                    "forcenomail",
                    new HelpMessage(
                        this.CommandName,
                        "forcenomail <account>",
                        "Forcibly removes a user's email address"));
            }

            return help;
        }

        private IEnumerable<CommandResponse> GetBotUser(out IBotUser botUser)
        {
            botUser = null;

            if (this.User.Account == null)
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Please identify to NickServ first!",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            var accountKey = string.Format("$a:{0}", this.User.Account);

            botUser = this.botUserConfiguration[accountKey];

            if (botUser == null)
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "You are not registered with the bot!",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }

            return null;
        }
    }
}