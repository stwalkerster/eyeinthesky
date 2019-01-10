namespace EyeInTheSky.Services.Interfaces
{
    using EyeInTheSky.Model.Interfaces;

    public interface IRecentChangeParser
    {
        IRecentChange Parse(string data, string channel);
    }
}