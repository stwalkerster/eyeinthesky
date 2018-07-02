namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using System.Xml;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    public interface IStalk : INamedItem
    {
        DateTime? LastUpdateTime { get; }
        DateTime? LastTriggerTime { get; set; }
        bool IsEnabled { get; set; }
        bool MailEnabled { get; set; }
        string Description { get; set; }
        DateTime? ExpiryTime { get; set; }
        IStalkNode SearchTree { get; set; }
        int TriggerCount { get; set; }
        string LastMessageId { get; set; }
        string Channel { get; set; }
        bool IsActive();
        bool Match(IRecentChange rc);
    }
}