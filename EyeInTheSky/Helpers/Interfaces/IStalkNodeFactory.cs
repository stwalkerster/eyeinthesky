using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Helpers.Interfaces
{
    public interface IStalkNodeFactory
    {
        IStalkNode NewFromXmlFragment(XmlElement fragment);
    }
}