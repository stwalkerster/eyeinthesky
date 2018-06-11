namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    [CommandInvocation("xml")]
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Standard)]
    public class XmlCommand : CommandBase
    {
        private readonly IXmlCacheService xmlCacheService;

        public XmlCommand(string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IXmlCacheService xmlCacheService) : base(
            commandSource,
            user,
            arguments,
            logger,
            flagService,
            configurationProvider,
            client)
        {
            this.xmlCacheService = xmlCacheService;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            this.xmlCacheService.CacheXml(this.OriginalArguments, this.User);
            yield return new CommandResponse {Message = "Cached requested XML."};
        }
        

        protected override IDictionary<string, HelpMessage> Help()
        {
            return new Dictionary<string, HelpMessage>
            {
                {
                    string.Empty,
                    new HelpMessage(
                        this.CommandName,
                        "<xml>",
                        "Caches some XML for use in other commands")
                }
            };
        }
    }
}