namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    public interface IStalk : INamedItem
    {
        DateTime? LastUpdateTime { get; }
        DateTime? LastTriggerTime { get; set; }
        bool IsEnabled { get; set; }
        string Description { get; set; }
        DateTime? ExpiryTime { get; set; }
        TimeSpan? DynamicExpiry { get; set; }
        IStalkNode SearchTree { get; set; }
        int TriggerCount { get; set; }
        string LastMessageId { get; set; }
        string Channel { get; set; }
        bool IsActive();
        bool IsExpiringSoon();
        bool Match(IRecentChange rc);
        List<StalkUser> Subscribers { get; }
        string WatchChannel { get; set; }
        DateTime CreationDate { get; }
        List<TimeSpan> ExecutionHistory { get; }
        bool TriggerDynamicExpiry();
    }
}