namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class TemplateConfiguration : ConfigFileBase<ITemplate>, ITemplateConfiguration
    {
        private readonly ILogger logger;
        private readonly ICommandParser commandParser;

        public TemplateConfiguration(IAppConfiguration appConfig,
            ILogger logger,
            ITemplateFactory factory,
            ICommandParser commandParser)
            : base(appConfig.TemplateConfigFile, "templates", logger, factory.NewFromXmlElement, factory.ToXmlElement)
        {
            this.logger = logger;
            this.commandParser = commandParser;
        }

        public IStalk NewFromTemplate(string flag, ITemplate template, IList<string> parameters)
        {
            var stalkNode = (IStalkNode)template.SearchTree.Clone();
            this.ApplyTemplate(stalkNode, parameters);
                
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