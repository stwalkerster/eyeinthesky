﻿namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Logging;
    using EyeInTheSky.Extensions;
    using EyeInTheSky.Model;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Attributes;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Models;
    using Stwalkerster.Bot.CommandLib.Commands.CommandUtilities.Response;
    using Stwalkerster.Bot.CommandLib.Exceptions;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;
    
    [CommandFlag(AccessFlags.Configuration)]
    public class AddTemplatedStalkCommand : CommandBase
    {
        private readonly ITemplateConfiguration templateConfiguration;

        private readonly IChannelConfiguration channelConfiguration;

        public AddTemplatedStalkCommand(
            string commandSource,
            IUser user,
            IList<string> arguments,
            ILogger logger,
            IFlagService flagService,
            IConfigurationProvider configurationProvider,
            IIrcClient client,
            ITemplateConfiguration templateConfiguration,
            IChannelConfiguration channelConfiguration)
            : base(commandSource,
                user,
                arguments,
                logger,
                flagService,
                configurationProvider,
                client)
        {
            this.templateConfiguration = templateConfiguration;
            this.channelConfiguration = channelConfiguration;
        }

        protected override IEnumerable<CommandResponse> Execute()
        {
            if (!this.CommandSource.StartsWith("#"))
            {
                throw new CommandErrorException("This command must be executed in-channel!");
            }
            
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

            string proposedFlag = null;
            if (template.StalkFlag == null)
            {
                proposedFlag = args.PopFromFront();    
            }
            
            var newStalk = this.templateConfiguration.NewFromTemplate(proposedFlag, template, args);
            newStalk.Channel = this.CommandSource;

            if (this.channelConfiguration[this.CommandSource].Stalks.ContainsKey(newStalk.Identifier))
            {
                throw new CommandErrorException(
                    string.Format("A stalk with flag {0} already exists!", this.InvokedAs));
            }
            
            this.channelConfiguration[this.CommandSource].Stalks.Add(newStalk.Identifier, newStalk);
            this.channelConfiguration.Save();

            yield return new CommandResponse
            {
                Message = string.Format(
                    "Created new stalk {1} using template {0} with CSL value: {2}",
                    this.InvokedAs,
                    newStalk.Identifier,
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
                        string.Empty,
                        flag + "<parameters...>",
                        "Defines a new stalk using the template " + this.InvokedAs)
                }
            };
        }
    }
}