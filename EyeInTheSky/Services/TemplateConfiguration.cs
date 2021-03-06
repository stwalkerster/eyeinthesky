﻿namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Commands;
    using EyeInTheSky.Formatters;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class TemplateConfiguration : ConfigFileBase<ITemplate>, ITemplateConfiguration
    {
        private readonly ICommandParser commandParser;
        private readonly IStalkNodeFactory stalkNodeFactory;

        public TemplateConfiguration(
            IAppConfiguration appConfig,
            ILogger logger,
            ITemplateFactory factory,
            ICommandParser commandParser,
            IStalkNodeFactory stalkNodeFactory,
            IFileService fileService)
            : base(appConfig.TemplateConfigFile,
                "templates",
                logger,
                factory.NewFromXmlElement,
                factory.ToXmlElement,
                fileService)
        {
            this.commandParser = commandParser;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        public IStalk NewFromTemplate(string flag, ITemplate template, IList<string> parameters)
        {
            var formatParameters = parameters.ToArray<object>();
            var templateSearchTree = string.Format(new StalkConfigFormatter(), template.SearchTree, formatParameters);

            var doc = new XmlDocument();
            doc.LoadXml(templateSearchTree);

            var stalkNode = this.stalkNodeFactory.NewFromXmlFragment(doc.DocumentElement);

            if (flag == null)
            {
                if (template.StalkFlag == null)
                {
                    throw new Exception("Cannot create stalk without defined flag");
                }

                flag = string.Format(template.StalkFlag, formatParameters);
            }


            string description = null;
            if (template.Description != null)
            {
                description = string.Format(template.Description, formatParameters);
            }

            var stalk = new ComplexStalk(flag)
            {
                Description = description,
                IsEnabled = template.StalkIsEnabled,
                ExpiryTime = template.ExpiryDuration.HasValue ? DateTime.UtcNow + template.ExpiryDuration : null,
                SearchTree = stalkNode,
                WatchChannel = template.WatchChannel
            };

            return stalk;
        }

        protected override void LocalInitialise()
        {
            List<ITemplate> enabled = new List<ITemplate>();
            bool dirty = false;
            foreach (var template in this.ItemList.Where(x => x.Value.TemplateIsEnabled).Select(x => x.Value))
            {

                if (this.commandParser.GetRegisteredCommand(template.Identifier) != null)
                {
                    this.Logger.ErrorFormat("{0} is already registered as a command, disabling!", template.Identifier);
                    template.TemplateIsEnabled = false;
                    dirty = true;
                }
                else
                {
                    enabled.Add(template);
                }
            }

            if (dirty)
            {
                this.Save();
            }

            foreach (var template in enabled)
            {
                this.commandParser.RegisterCommand(template.Identifier, typeof(AddTemplatedStalkCommand));
            }
        }

        protected override void OnAdd(ITemplate template)
        {
            if (this.commandParser.GetRegisteredCommand(template.Identifier) != null)
            {
                this.Logger.ErrorFormat("{0} is already registered as a command, disabling!", template.Identifier);
                template.TemplateIsEnabled = false;
            }
            else
            {
                this.commandParser.RegisterCommand(template.Identifier, typeof(AddTemplatedStalkCommand));
            }

        }

        protected override void OnRemove(ITemplate item)
        {
            if (this.commandParser.GetRegisteredCommand(item.Identifier) == typeof(AddTemplatedStalkCommand))
            {
                this.commandParser.UnregisterCommand(item.Identifier);
            }
        }
    }
}