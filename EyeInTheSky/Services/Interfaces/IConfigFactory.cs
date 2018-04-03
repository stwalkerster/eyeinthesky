namespace EyeInTheSky.Services.Interfaces
{
    using System.Xml;
    
    public interface IConfigFactory<T>
    {
        T NewFromXmlElement(XmlElement element);
        XmlElement ToXmlElement(T item, XmlDocument doc);
    }
}