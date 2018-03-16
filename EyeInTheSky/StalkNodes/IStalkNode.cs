namespace EyeInTheSky.StalkNodes
{
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkNode
    {
        bool Match(IRecentChange rc);
        XmlElement ToXmlFragment(XmlDocument doc, string xmlns);
    }
}