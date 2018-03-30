namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;

    public interface IConfigurationBase<T>
    {
        IReadOnlyList<T> Items { get; }
        T this[string stalkName] { get; }
        void Add(string key, T stalk);
        void Remove(string key);
        void Save();
        bool ContainsKey(string stalkName);
    }
}