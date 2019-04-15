namespace EyeInTheSky.Services.ExternalProviders.Interfaces
{
    using System;
    using System.Xml;

    using EyeInTheSky.Model.StalkNodes;

    public interface IExternalProvider
    {
        [Obsolete]
        XmlElement GetFragmentFromSource(string location);
        XmlElement PopulateFromExternalSource(ExternalNode externalNode);
    }
}