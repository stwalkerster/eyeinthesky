namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;

    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration(
            string freenodeChannel,
            string wikimediaChannel,
            string commandPrefix,
            string userConfigFile,
            string channelConfigFile,
            string templateConfigFile,
            string rcUser,
            string dateFormat,
            string owner,
            string mediaWikiApiEndpoint,
            string userAgent)
        {
            if (freenodeChannel == null)
            {
                throw new ArgumentNullException("freenodeChannel");
            }

            if (wikimediaChannel == null)
            {
                throw new ArgumentNullException("wikimediaChannel");
            }

            if (commandPrefix == null)
            {
                throw new ArgumentNullException("commandPrefix");
            }

            if (userConfigFile == null)
            {
                throw new ArgumentNullException("userConfigFile");
            }

            if (channelConfigFile == null)
            {
                throw new ArgumentNullException("channelConfigFile");
            }

            if (templateConfigFile == null)
            {
                throw new ArgumentNullException("templateConfigFile");
            }

            if (rcUser == null)
            {
                throw new ArgumentNullException("rcUser");
            }

            if (dateFormat == null)
            {
                throw new ArgumentNullException("dateFormat");
            }

            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (mediaWikiApiEndpoint == null)
            {
                throw new ArgumentNullException("mediaWikiApiEndpoint");
            }

            if (userAgent == null)
            {
                throw new ArgumentNullException("userAgent");
            }

            this.FreenodeChannel = freenodeChannel;
            this.WikimediaChannel = wikimediaChannel;
            this.CommandPrefix = commandPrefix;
            this.UserConfigFile = userConfigFile;
            this.ChannelConfigFile = channelConfigFile;
            this.TemplateConfigFile = templateConfigFile;
            this.RcUser = rcUser;
            this.Owner = owner;
            this.DateFormat = dateFormat;
            this.MediaWikiApiEndpoint = mediaWikiApiEndpoint;
            this.UserAgent = userAgent;
        }

        public string FreenodeChannel { get; private set; }
        public string WikimediaChannel { get; private set; }
        public string CommandPrefix { get; private set; }
        public string UserConfigFile { get; private set; }
        public string ChannelConfigFile { get; private set; }
        public string TemplateConfigFile { get; private set; }
        public string RcUser { get; private set; }
        public string Owner { get; private set; }
        public string DateFormat { get; private set; }
        public string MediaWikiApiEndpoint { get; private set; }
        public string UserAgent { get; private set; }

        // These properties are optional, and set by Castle
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public EmailConfiguration EmailConfiguration { get; set; }
        public int MonitoringPort { get; set; }
        public string PhabUrl { get; set; }
        public string PhabToken { get; set; }

        public string PrivacyPolicy { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        // These properties are deprecated and used only by migrations.
        // They are also optional, and also set by Castle
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string StalkConfigFile { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}