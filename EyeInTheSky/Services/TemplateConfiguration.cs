using System.Linq;

namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Castle.Core.Logging;
    using EyeInTheSky.Commands;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class TemplateConfiguration : ConfigFileBase<ITemplate>, ITemplateConfiguration
    {
        private readonly ILogger logger;
        private readonly ICommandParser commandParser;
        private readonly IStalkNodeFactory stalkNodeFactory;

        public TemplateConfiguration(IAppConfiguration appConfig,
            ILogger logger,
            ITemplateFactory factory,
            ICommandParser commandParser,
            IStalkNodeFactory stalkNodeFactory)
            : base(appConfig.TemplateConfigFile, "templates", logger, factory.NewFromXmlElement, factory.ToXmlElement)
        {
            this.logger = logger;
            this.commandParser = commandParser;
            this.stalkNodeFactory = stalkNodeFactory;
        }

        public IStalk NewFromTemplate(string flag, ITemplate template, IList<string> parameters)
        {
            var templateSearchTree = string.Format(template.SearchTree, parameters.ToArray());
            
            var doc =new XmlDocument();
            doc.LoadXml(templateSearchTree);

            var stalkNode = this.stalkNodeFactory.NewFromXmlFragment(doc.DocumentElement);
            
            if (flag == null)
            {
                if (template.StalkFlag == null)
                {
                    throw new Exception("Cannot create stalk without defined flag");
                }

                flag = string.Format(template.StalkFlag, parameters);
            }
            
            var stalk = new ComplexStalk(flag)
            {
                Description = string.Format(template.Description, parameters),
                IsEnabled = template.StalkIsEnabled,
                ExpiryTime = template.ExpiryDuration.HasValue ? DateTime.Now + template.ExpiryDuration : null,
                MailEnabled = template.MailEnabled,
                SearchTree = stalkNode
            };

            return stalk;
        }

        protected override void LocalInitialise()
        {
            List<ITemplate> enabled = new List<ITemplate>();
            bool dirty = false;
            foreach (var template in this.ItemList.Where(x => x.Value.TemplateIsEnabled).Select(x => x.Value))
            {
                
                if (this.commandParser.GetRegisteredCommand(template.Flag) != null)
                {
                    this.logger.ErrorFormat("{0} is already registered as a command, disabling!", template.Flag);
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
                this.commandParser.RegisterCommand(template.Flag, typeof(AddTemplatedStalkCommand));
            }
        }

        protected override void OnAdd(ITemplate template)
        {
            if (this.commandParser.GetRegisteredCommand(template.Flag) != null)
            {
                this.logger.ErrorFormat("{0} is already registered as a command, disabling!", template.Flag);
                template.TemplateIsEnabled = false;
            }
            else
            {
                this.commandParser.RegisterCommand(template.Flag, typeof(AddTemplatedStalkCommand));
            }
            
        }

        protected override void OnRemove(ITemplate item)
        {
            if (this.commandParser.GetRegisteredCommand(item.Flag) == typeof(AddTemplatedStalkCommand))
            {
                this.commandParser.UnregisterCommand(item.Flag);
            }
        }

        private void ApplyTemplate(IStalkNode root, IList<string> parameters)
        {
            var leafNode = root as LeafNode;
            if (leafNode != null)
            {
                leafNode.SetMatchExpression(string.Format(leafNode.GetMatchExpression(), parameters));
                return;
            }

            var scln = root as SingleChildLogicalNode;
            if (scln != null)
            {
                this.ApplyTemplate(scln.ChildNode, parameters);
            }

            var dcln = root as DoubleChildLogicalNode;
            if (dcln != null)
            {
                this.ApplyTemplate(dcln.LeftChildNode, parameters);
                this.ApplyTemplate(dcln.RightChildNode, parameters);
            }

            var mcln = root as MultiChildLogicalNode;
            if (mcln != null)
            {
                foreach (var child in mcln.ChildNodes)
                {
                    this.ApplyTemplate(child, parameters);
                }
            }
        }
    }
}