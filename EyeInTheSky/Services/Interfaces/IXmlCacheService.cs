namespace EyeInTheSky.Services.Interfaces
{
    using System.Xml;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public interface IXmlCacheService
    {
        void CacheXml(string xml, IUser forUser);
        XmlElement RetrieveXml(IUser forUser);
    }
}