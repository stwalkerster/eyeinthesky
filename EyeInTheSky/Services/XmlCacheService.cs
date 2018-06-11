namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Xml;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class XmlCacheService : IXmlCacheService
    {
        private readonly Dictionary<IUser, XmlElement> cache = new Dictionary<IUser, XmlElement>();
        
        public void CacheXml(string xml, IUser forUser)
        {
            var xd = new XmlDocument();
            xd.LoadXml(xml);
            
            lock (this.cache)
            {
                if (this.cache.ContainsKey(forUser))
                {
                    this.cache.Remove(forUser);
                }
                
                this.cache.Add(forUser, (XmlElement)xd.FirstChild);
            }
        }

        public XmlElement RetrieveXml(IUser forUser)
        {
            XmlElement element;
            if (!this.cache.TryGetValue(forUser, out element))
            {
                return null;
            }

            return element;
        }
    }
}