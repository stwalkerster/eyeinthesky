using System.Xml;

namespace EyeInTheSky.StalkNodes
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    class TrueNode : LogicalNode
    {
        public override bool Match(IRecentChange rc)
        {
            if (rc == null)
            {
                throw new ArgumentNullException("rc");
            }
            
            return true;
        }

        public override XmlElement ToXmlFragment(XmlDocument doc, string xmlns)
        {
            return doc.CreateElement("true", xmlns);
        }

        public override string ToString()
        {
            return "(true)";
        }
    }
}
