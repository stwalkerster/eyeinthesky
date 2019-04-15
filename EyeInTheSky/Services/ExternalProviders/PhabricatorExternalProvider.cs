namespace EyeInTheSky.Services.ExternalProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Services.ExternalProviders.Interfaces;

    using Stwalkerster.SharphConduit;
    using Stwalkerster.SharphConduit.Applications.Paste;

    public class PhabricatorExternalProvider : IPhabricatorExternalProvider
    {
        private readonly bool active;
        private readonly Paste paste;

        public PhabricatorExternalProvider(IAppConfiguration appConfig)
        {
            var url = appConfig.PhabUrl;
            var key = appConfig.PhabToken;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                this.active = false;
                return;
            }
            
            this.active = true;
            
            var conduitClient = new ConduitClient(url, key);
            this.paste = new Paste(conduitClient);
        }

        public XmlElement PopulateFromExternalSource(ExternalNode node)
        {
            if (!this.active)
            {
                throw new InvalidOperationException(
                    "Cannot retrieve external resources from Phabricator as this is not configured.");
            }

            string location = node.Location;
            
            if (node.Location.StartsWith("P"))
            {
                location = node.Location.Substring(1);
            }

            var pasteId = new ApplicationEditorSearchConstraint("ids", new List<int> {int.Parse(location)});
            var pasteItems = this.paste.Search(constraints: new[] {pasteId}, attachments: new[] {"content"});

            var document = new XmlDocument();
            var pasteItem = pasteItems.First();
                       
            document.LoadXml(pasteItem.Text);
            node.Comment = pasteItem.Title;
            
            return document.DocumentElement;
        }
        
        public XmlElement GetFragmentFromSource(string location)
        {
            if (!this.active)
            {
                throw new InvalidOperationException(
                    "Cannot retrieve external resources from Phabricator as this is not configured.");
            }

            if (location.StartsWith("P"))
            {
                location = location.Substring(1);
            }

            var pasteId = new ApplicationEditorSearchConstraint("ids", new List<int> {int.Parse(location)});
            var pasteItems = this.paste.Search(constraints: new[] {pasteId}, attachments: new[] {"content"});

            var document = new XmlDocument();
            document.LoadXml(pasteItems.First().Text);
            return document.DocumentElement;
        }
    }
}