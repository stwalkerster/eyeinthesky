namespace EyeInTheSky.Model.Interfaces
{
    using System;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    
    public interface ITemplate : INamedItem
    {
        DateTime? LastUpdateTime { get; }
        bool TemplateIsEnabled { get; set; }
        bool StalkIsEnabled { get; set; }
        string Description { get; set; }
        TimeSpan? ExpiryDuration { get; set; }
        string SearchTree { get; set; }
        string StalkFlag { get; set; }
        string WatchChannel { get; set; }
    }
}