using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Helpers.Interfaces
{
    public interface IStalkNodeFactory
    {
        IStalkNode NewFromXmlFragment(XmlElement fragment);
        XmlElement ToXml(XmlDocument doc, string xmlns, IStalkNode node);
    }
}