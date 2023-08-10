namespace EyeInTheSky.Services
{
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IAppConfiguration appConfiguration;

        public ConfigurationProvider(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public string CommandPrefix
        {
            get { return this.appConfiguration.CommandPrefix; }
        }

        public string DebugChannel
        {
            get { return this.appConfiguration.FreenodeChannel; }
        }

        public bool AllowQuotedStrings => false;
    }
}