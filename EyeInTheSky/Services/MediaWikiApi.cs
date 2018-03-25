namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Xml.XPath;
    using Castle.Core.Logging;
    using EyeInTheSky.Services.Interfaces;

    public class MediaWikiApi : IMediaWikiApi
    {
        private readonly ILogger logger;
        private readonly IWebServiceClient wsClient;

        private readonly Dictionary<string, List<string>> rightsCache;
        
        public MediaWikiApi(ILogger logger, IWebServiceClient wsClient)
        {
            this.logger = logger;
            this.wsClient = wsClient;
            
            this.rightsCache = new Dictionary<string, List<string>>();
        }

        public IEnumerable<string> GetUserGroups(string user)
        {
            if (this.rightsCache.ContainsKey(user))
            {
                this.logger.DebugFormat("Getting groups for {0} from cache", user);
                return this.rightsCache[user];
            }
            
            this.logger.InfoFormat("Getting groups for {0} from webservice", user);
                
            var queryparams = new NameValueCollection
            {
                {"action", "query"},
                {"list", "users"},
                {"usprop", "groups"},
                {"ususers", user}
            };

            var userGroups = this.GetGroups(this.wsClient.DoApiCall(queryparams)).ToList();
            this.rightsCache.Add(user, userGroups);
            
            return userGroups;
        }

        public bool PageIsInCategory(string page, string category)
        {
            this.logger.InfoFormat("Getting category {1} for {0} from webservice", page, category);
            
            var queryparams = new NameValueCollection
            {
                {"action", "query"},
                {"prop", "categories"},
                {"titles", page},
                {"clcategories", category},
            };

            var result = this.GetCategories(this.wsClient.DoApiCall(queryparams)).ToList();
            return result.Any();
        }

        private IEnumerable<string> GetCategories(Stream apiResult)
        {
            var nav = new XPathDocument(apiResult).CreateNavigator();
            var groups = new List<string>();
            foreach (var node in nav.Select("//categories/cl/@title"))
            {
                groups.Add(node.ToString());
            }

            return groups;
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