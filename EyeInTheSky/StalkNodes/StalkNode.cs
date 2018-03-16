using System;
using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Model.Interfaces;

    public abstract class StalkNode : IStalkNode
    {
        public abstract bool Match(IRecentChange rc);

        public abstract XmlElement ToXmlFragment(XmlDocument doc, string xmlns);
    }
}
