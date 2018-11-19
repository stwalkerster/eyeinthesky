namespace EyeInTheSky.Services.ExternalProviders.Interfaces
{
    using System.Xml;

    public interface IExternalProvider
    {
        XmlElement GetFragmentFromSource(string location);
    }
}