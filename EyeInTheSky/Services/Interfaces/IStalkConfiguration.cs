namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkConfiguration
    {
        IReadOnlyList<IStalk> StalkList { get; }
        IStalk this[string stalkName] { get; }
        void Add(string key, IStalk stalk);
        void Remove(string key);
        void Save();
        IEnumerable<IStalk> MatchStalks(IRecentChange rc);
        bool ContainsKey(string stalkName);
    }
}