namespace EyeInTheSky.Services.Interfaces
{
    using System.Collections.Generic;

    public interface IMediaWikiApi
    {
        IEnumerable<string> GetUserGroups(string user);
    }
}