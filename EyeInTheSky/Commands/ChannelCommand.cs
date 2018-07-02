namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    using CLFlag = Stwalkerster.Bot.CommandLib.Model.Flag;

    [CommandInvocation("channel")]
    public class ChannelCommand : CommandBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IAppConfiguration appConfig;

        public ChannelCommand(
            string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IChannelConfiguration channelConfiguration,
            IAppConfiguration appConfig)
            : base(commandSource, user, arguments, logger, flagService, configurationProvider, client)
        {
            this.channelConfiguration = channelConfiguration;
            this.appConfig = appConfig;
        }

        public override bool CanExecute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                return base.CanExecute();
            }

            var mode = tokenList.PopFromFront();
            var channel = tokenList.PopFromFront();

            switch (mode)
            {
                case "join":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null);
                case "part":
                    return this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null) ||
                           this.FlagService.UserHasFlag(this.User, AccessFlags.ChannelAdmin, channel);
                default:
                    return false;
            }
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var tokenList = this.OriginalArguments.ToParameters().ToList();

            if (tokenList.Count < 1)
            {
                throw new ArgumentCountException(2, this.Arguments.Count());
            }

            var mode = tokenList.PopFromFront();
            var channel = tokenList.PopFromFront();

            switch (mode)
            {
                case "join":
                    return this.JoinMode(channel);
                case "part":
                    return this.PartMode(channel);
                default:
                    throw new CommandInvocationException();
            }
        }

        private IEnumerable<CommandResponse> PartMode(string channel)
        {
            if (this.appConfig.FreenodeChannel == channel)
            {
                return new[]
                {
                    new CommandResponse
                    {
                        Message = "Cannot leave default channel"
                    }
                };
            }

            if (this.channelConfiguration.ContainsKey(channel))
            {
                this.channelConfiguration.Remove(channel);
                this.channelConfiguration.Save();
            }

            if (this.Client.Channels.ContainsKey(channel))
            {
                this.Client.PartChannel(channel, string.Format("requested by {0}", this.User.Nickname));
            }

            return null;
        }

        private IEnumerable<CommandResponse> JoinMode(string channel)
        {
            this.channelConfiguration.Add(new IrcChannel(channel));
            this.channelConfiguration.Save();
            this.Client.JoinChannel(channel);
            this.Client.SendMessage(
                channel,
                string.Format("My presence in {0} was requested by {1}.", channel, this.User));

            yield break;
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            var help = new Dictionary<string, HelpMessage>();

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.GlobalAdmin, null))
            {
                help.Add(
                    "join",
                    new HelpMessage(
                        this.CommandName,
                        "join <channel>",
                        "Requests the bot joins a channel"));

                help.Add(
                    "part",
                    new HelpMessage(
                        this.CommandName,
                        "part <channel>",
                        "Requests the bot leaves a channel, removing all configuration for that channel."));
            }

            if (this.FlagService.UserHasFlag(this.User, AccessFlags.ChannelAdmin, this.CommandSource))
            {
                if (!help.ContainsKey("part"))
                {
                    help.Add(
                        "part",
                        new HelpMessage(
                            this.CommandName,
                            "part <channel>",
                            "Requests the bot leaves a channel, removing all configuration for that channel."));
                }
            }

            return help;
        }
    }
}