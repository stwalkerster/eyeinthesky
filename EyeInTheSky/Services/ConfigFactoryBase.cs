namespace EyeInTheSky.Services
{
    using System;
    using System.Globalization;
    using System.Xml;
    using Castle.Core.Logging;

    public abstract class ConfigFactoryBase
    {
        public ConfigFactoryBase(ILogger logger)
        {
            this.Logger = logger;
        }
        protected ILogger Logger { get; private set; }

        internal DateTime ParseDate(string flagName, string input, string propName)
        {
            DateTime result;

            try
            {
                result = XmlConvert.ToDateTime(input, XmlDateTimeSerializationMode.Utc);
            }
            catch (FormatException ex)
            {
                this.Logger.WarnFormat("Unknown date format in item '{0}' {2}: {1}", flagName, input, propName);

                if (!DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    var err = string.Format("Failed date parse for item '{0}' {2}: {1}", flagName, input, propName);

                    this.Logger.Error(err);
                    throw new FormatException(err, ex);
                }
            }

            return result;
        }
    }
}