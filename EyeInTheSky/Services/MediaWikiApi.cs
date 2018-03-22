namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using EyeInTheSky.Services.Interfaces;

    public class MediaWikiApi : IMediaWikiApi
    {
        private readonly ILogger logger;
        private readonly IWebServiceClient wsClient;

        public MediaWikiApi(ILogger logger, IWebServiceClient wsClient)
        {
            this.logger = logger;
            this.wsClient = wsClient;
        }

        public IEnumerable<string> GetUserGroups(string user)
        {
            this.logger.InfoFormat("Getting groups for {0}", user);
                
            var queryparams = new NameValueCollection
            {
                {"action", "query"},
                {"list", "users"},
                {"usprop", "groups"},
                {"ususers", user}
            };

            return this.GetGroups(this.wsClient.DoApiCall(queryparams));
        }
        
        private IEnumerable<string> GetGroups(Stream apiResult)
        {
            var nav = new XPathDocument(apiResult).CreateNavigator();
            if (nav.SelectSingleNode("//user/@invalid") != null || nav.SelectSingleNode("//user/@missing") != null)
            {
                return new List<string> {"*"};
            }

            var groups = new List<string>();
            foreach (var node in nav.Select("//user/groups/g"))
            {
                groups.Add(node.ToString());
            }

            return groups;
        }
    }
}