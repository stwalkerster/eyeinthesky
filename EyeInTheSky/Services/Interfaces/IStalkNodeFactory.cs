namespace EyeInTheSky.Services.Interfaces
{
    using System.Xml;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    public interface IStalkNodeFactory
    {
        IStalkNode NewFromXmlFragment(XmlElement fragment);
        XmlElement ToXml(XmlDocument doc, IStalkNode node);
    }
}