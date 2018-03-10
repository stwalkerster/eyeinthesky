namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using System.Xml;
    using EyeInTheSky.StalkNodes;

    public interface IStalk
    {
        string Flag { get; }
        DateTime LastUpdateTime { get; }
        DateTime LastTriggerTime { get; set; }
        bool IsEnabled { get; set; }
        bool MailEnabled { get; set; }
        string Description { get; set; }
        DateTime ExpiryTime { get; set; }
        StalkNode SearchTree { get; set; }
        bool IsActive();
        bool Match(IRecentChange rc);
        XmlElement ToXmlFragment(XmlDocument doc, string xmlns);
    }
}