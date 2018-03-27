namespace EyeInTheSky.Services.Interfaces
{
    using System.Xml;
    using EyeInTheSky.StalkNodes;

    public interface IStalkNodeFactory
    {
        IStalkNode NewFromXmlFragment(XmlElement fragment);
        XmlElement ToXml(XmlDocument doc, string xmlns, IStalkNode node);
    }
}