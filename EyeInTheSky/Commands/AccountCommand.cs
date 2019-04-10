namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using BCrypt.Net;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using MimeKit;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("account")]
    [CommandFlag(AccessFlags.User)]
    public class AccountCommand : CommandBase
    {
        private readonly IAppConfiguration appConfig;
        private readonly INotificationTemplates templates;
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IEmailHelper emailHelper;

        public AccountCommand(string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IBotUserConfiguration botUserConfiguration,
            IEmailHelper emailHelper,
            IAppConfiguration config,
            INotificationTemplates templates,
            IChannelConfiguration channelConfiguration) : base(
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
            this.channelConfiguration = channelConfiguration;
            this.botUserConfiguration = botUserConfiguration;
            this.emailHelper = emailHelper;
        }

        [SubcommandInvocation("verify")]
        [RequiredArguments(1)]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> VerifyMode()
        {
            var tokenList = this.Arguments;
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

        [SubcommandInvocation("email")]
        [RequiredArguments(1)]
        [Help(new[] {"<address>", "none"}, "Sets or removes your email address")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> EmailMode()
        {
            IList<string> tokenList = this.Arguments;
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
                botUser, null);

            return new[]
            {
                new CommandResponse
                {
                    Message = "Please check your email for further instructions.",
                    Destination = CommandResponseDestination.PrivateMessage
                }
            };
        }

        [SubcommandInvocation("deleteconfirm")]
        [RequiredArguments(1)]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DeleteConfirmMode()
        {
            IList<string> tokenList = this.Arguments;
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
                    this.RemoveAccount(botUser.Identifier);

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

        [SubcommandInvocation("forcedelete")]
        [RequiredArguments(1)]
        [CommandFlag(AccessFlags.GlobalAdmin)]
        [Help("<account>", "Forcibly deletes a user's account")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ForceDeleteMode()
        {
            IList<string> tokenList = this.Arguments;
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

            this.RemoveAccount(accountKey);

            yield return new CommandResponse
            {
                Message = "Deleted user " + accountKey
            };
        }

        [SubcommandInvocation("webpass")]
        [Help("[password]", "Sets or removes your web password")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> WebPasswordMode()
        {
            IBotUser botUser;
            var resp = this.GetBotUser(out botUser);
            if (resp != null)
            {
                return resp;
            }

            IList<string> tokenList = this.Arguments;
            var password = tokenList.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(password))
            {
                botUser.WebPassword = null;
                this.botUserConfiguration.Save();

                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Your web password has been removed.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }
            else
            {
                botUser.WebPassword = BCrypt.HashPassword(password);
                this.botUserConfiguration.Save();

                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Your web password has been set.",
                        Destination = CommandResponseDestination.PrivateMessage
                    }
                };
            }
        }

        [SubcommandInvocation("forcenomail")]
        [RequiredArguments(1)]
        [CommandFlag(AccessFlags.GlobalAdmin)]
        [Help("<account>", "Forcibly removes a user's email address")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> ForceNoMailMode()
        {
            IList<string> tokenList = this.Arguments;
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

        [SubcommandInvocation("delete")]
        [Help("", "Deletes your account with the bot")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> DeleteMode()
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
                    botUser, null);

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

        [SubcommandInvocation("register")]
        [Help("", "Registers your NickServ account with the bot.")]
        // ReSharper disable once UnusedMember.Global
        protected IEnumerable<CommandResponse> RegisterMode()
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
                Message = string.Format("Registered using NickServ account {0}.", this.User.Account),
                Destination = CommandResponseDestination.PrivateMessage
            };
            yield return new CommandResponse
            {
                Message = string.Format(
                    "Please note, to receive notifications via email, you need to provide your email address (=account email <address>). By providing your email address, you agree to the privacy policy: {0}",
                    this.appConfig.PrivacyPolicy),
                Destination = CommandResponseDestination.PrivateMessage
            };
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            throw new CommandInvocationException();
        }

        private IEnumerable<CommandResponse> GetBotUser(out IBotUser botUser)
        {
            botUser = null;

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

        private void RemoveAccount(string accountKey)
        {
            this.botUserConfiguration.Remove(accountKey);
            this.botUserConfiguration.Save();

            foreach (var channel in this.channelConfiguration.Items)
            {
                channel.Users.RemoveAll(x => x.Mask.ToString() == accountKey);
            }

            this.channelConfiguration.Save();
        }
    }
}