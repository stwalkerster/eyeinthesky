using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    public abstract class StalkNode : IStalkNode
    {
        public abstract bool Match(IRecentChange rc);

        [Obsolete("Use StalkNodeFactory instead", true)]
        public static IStalkNode NewFromXmlFragment(XmlNode xmlNode)
        {
            return null;
        }

        public abstract XmlElement ToXmlFragment(XmlDocument doc, string xmlns);
    }
}
