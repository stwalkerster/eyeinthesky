namespace EyeInTheSky.Services.Interfaces
{
    using EyeInTheSky.Model;

    public interface IRecentChangeParser
    {
        RecentChange Parse(string data);
    }
}