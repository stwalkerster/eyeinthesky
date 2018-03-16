﻿namespace EyeInTheSky.Helpers.Interfaces
{
    using System.Xml;
    using EyeInTheSky.Model.Interfaces;

    public interface IStalkFactory
    {
        IStalk NewFromXmlElement(XmlElement element);
        XmlElement ToXmlElement(IStalk stalk, XmlDocument doc, string xmlns);
    }
}