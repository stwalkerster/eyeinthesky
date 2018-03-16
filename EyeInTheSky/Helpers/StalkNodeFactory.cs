using System.Xml;
using EyeInTheSky.Helpers.Interfaces;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Helpers
{
    public class StalkNodeFactory : IStalkNodeFactory
    {
        public IStalkNode NewFromXmlFragment(XmlElement fragment)
        {
            return StalkNode.NewFromXmlFragment(fragment);
        }
    }
}