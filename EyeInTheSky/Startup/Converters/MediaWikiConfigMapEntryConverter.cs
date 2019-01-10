namespace EyeInTheSky.Startup.Converters
{
    using System;
    using Castle.Core.Configuration;
    using Castle.MicroKernel.SubSystems.Conversion;
    using EyeInTheSky.Services.ConfigProviders;
    using Stwalkerster.Bot.MediaWikiLib.Configuration;

    public class MediaWikiConfigMapEntryConverter : AbstractTypeConverter
    {
        public override bool CanHandleType(Type type)
        {
            return type == typeof(MapMediaWikiConfigProvider.MapEntry);
        }

        public override object PerformConversion(string value, Type targetType)
        {
            throw new NotImplementedException();
        }

        public override object PerformConversion(IConfiguration configuration, Type targetType)
        {
            var channel = configuration.Attributes.Get("channel");
            var mediaWikiConfiguration =
                (MediaWikiConfiguration) configuration.GetValue(typeof(MediaWikiConfiguration), null);

            return new MapMediaWikiConfigProvider.MapEntry(channel, mediaWikiConfiguration);
        }
    }
}