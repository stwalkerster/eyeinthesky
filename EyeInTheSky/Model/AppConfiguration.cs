namespace EyeInTheSky.Model
{
    using System;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class AppConfiguration : IAppConfiguration
    {
        public AppConfiguration(string freenodeChannel,
            string wikimediaChannel,
            string commandPrefix,
            string stalkConfigFile,
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

            if (stalkConfigFile == null)
            {
                throw new ArgumentNullException("stalkConfigFile");
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
            this.StalkConfigFile = stalkConfigFile;
            this.TemplateConfigFile = templateConfigFile;
            this.RcUser = IrcUser.FromPrefix(rcUser);
            this.Owner = IrcUser.FromPrefix(owner);
            this.DateFormat = dateFormat;
            this.MediaWikiApiEndpoint = mediaWikiApiEndpoint;
            this.UserAgent = userAgent;
        }

        public string FreenodeChannel { get; private set; }
        public string WikimediaChannel { get; private set; }
        public string CommandPrefix { get; private set; }
        public string StalkConfigFile { get; private set; }
        public string TemplateConfigFile { get; private set; }
        public IrcUser RcUser { get; private set; }
        public IrcUser Owner { get; private set; }
        public string DateFormat { get; private set; }
        public string MediaWikiApiEndpoint { get; private set; }
        public string UserAgent { get; private set; }
        
        // These properties are optional, and set by Castle
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public EmailConfiguration EmailConfiguration { get; set; }
        public int MonitoringPort { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }    
}