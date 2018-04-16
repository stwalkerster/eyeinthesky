namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    
    [CommandFlag(Stwalkerster.Bot.CommandLib.Model.Flag.Protected)]
    public class AddTemplatedStalkCommand : CommandBase
    {
        private readonly ITemplateConfiguration templateConfiguration;
        private readonly IStalkConfiguration stalkConfig;

        public AddTemplatedStalkCommand(
            string commandSource,
            IUser user,
            IEnumerable<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            IStalkConfiguration stalkConfig,
            ITemplateConfiguration templateConfiguration)
            : base(commandSource,
                user,
                arguments,
                logger,
                flagService,
                configurationProvider,
                client)
        {
            this.templateConfiguration = templateConfiguration;
            this.stalkConfig = stalkConfig;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            var args = this.OriginalArguments.ToParameters().ToList();
            
            if (args.Count < 2)
            {
                throw new ArgumentCountException(2, args.Count);
            }

            var template = this.templateConfiguration[this.InvokedAs];            
            if (template == null)
            {
                throw new CommandErrorException(
                    string.Format("Template {0} does not exist in configuration!", this.InvokedAs));
            }

            string flag = null;
            if (template.StalkFlag == null)
            {
                flag = args.PopFromFront();    
            }
            
            var newStalk = this.templateConfiguration.NewFromTemplate(flag, template, args);

            this.stalkConfig.Add(flag, newStalk);
            this.stalkConfig.Save();

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Created new stalk {1} using template {0} with CSL value: {2}",
                    this.InvokedAs,
                    flag,
                    newStalk.SearchTree)
            };
        }

        protected override IDictionary<string, HelpMessage> Help()
        {
            var template = this.templateConfiguration[this.InvokedAs];            
            if (template == null)
            {
                throw new CommandErrorException(
                    string.Format("Template {0} does not exist in configuration!", this.InvokedAs));
            }

            var flag = "<flag> ";
            if (template.StalkFlag != null)
            {
                flag = string.Empty;
            }
            
            return new Dictionary<string, HelpMessage>
            {
                {
                    this.InvokedAs,
                    new HelpMessage(
                        this.InvokedAs,
                        flag + "<parameters...>",
                        "Defines a new stalk using the template " + this.InvokedAs)
                }
            };
        }
    }
}