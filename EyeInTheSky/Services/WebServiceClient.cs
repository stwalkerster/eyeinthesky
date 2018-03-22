namespace EyeInTheSky.Services
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Castle.Core.Logging;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    
    public class WebServiceClient : IWebServiceClient
    {
        private readonly string endpoint;
        private readonly string userAgent;
        private readonly ILogger logger;

        private readonly object lockObject = new object();

        public WebServiceClient(IAppConfiguration appConfiguration, ILogger logger)
        {
            this.logger = logger;
            this.endpoint = appConfiguration.MediaWikiApiEndpoint;
            this.userAgent = appConfiguration.UserAgent;
        }

        public Stream DoApiCall(NameValueCollection query)
        {
            query.Set("format", "xml");

            var queryFragment = string.Join("&", query.AllKeys.Select(a => a + "=" + WebUtility.UrlEncode(query[a])));
            
            var url = string.Format("{0}?{1}", this.endpoint, queryFragment);
            
            this.logger.DebugFormat("Requesting {0}", url);
            
            var hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.UserAgent = this.userAgent;
            
            var memstream = new MemoryStream();

            lock (this.lockObject)
            {
                using (var resp = (HttpWebResponse) hwr.GetResponse())
                {
                    Stream responseStream = resp.GetResponseStream();

                    if (responseStream == null)
                    {
                        throw new NullReferenceException("Returned web request response stream was null.");
                    }

                    responseStream.CopyTo(memstream);
                    resp.Close();
                }
            }

            memstream.Position = 0;
            return memstream;
        }
    }
}