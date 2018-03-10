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
            string rcUser,
            string ircAlertFormat,
            string ircStalkTagSeparator,
            string emailRcTemplate,
            string emailStalkTemplate,
            string dateFormat,
            EmailConfiguration emailConfiguration)
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

            if (rcUser == null)
            {
                throw new ArgumentNullException("rcUser");
            }

            if (ircAlertFormat == null)
            {
                throw new ArgumentNullException("ircAlertFormat");
            }

            if (ircStalkTagSeparator == null)
            {
                throw new ArgumentNullException("ircStalkTagSeparator");
            }

            if (emailRcTemplate == null)
            {
                throw new ArgumentNullException("emailRcTemplate");
            }

            if (emailStalkTemplate == null)
            {
                throw new ArgumentNullException("emailStalkTemplate");
            }

            if (dateFormat == null)
            {
                throw new ArgumentNullException("dateFormat");
            }

            this.FreenodeChannel = freenodeChannel;
            this.WikimediaChannel = wikimediaChannel;
            this.CommandPrefix = commandPrefix;
            this.StalkConfigFile = stalkConfigFile;
            this.RcUser = IrcUser.FromPrefix(rcUser);
            this.IrcAlertFormat = ircAlertFormat;
            this.IrcStalkTagSeparator = ircStalkTagSeparator;
            this.EmailRcTemplate = emailRcTemplate;
            this.EmailStalkTemplate = emailStalkTemplate;
            this.DateFormat = dateFormat;
            this.EmailConfiguration = emailConfiguration;
        }

        public string FreenodeChannel { get; private set; }
        public string WikimediaChannel { get; private set; }
        public string CommandPrefix { get; private set; }
        public string StalkConfigFile { get; private set; }
        public IrcUser RcUser { get; private set; }
        public string IrcAlertFormat { get; private set; }
        public string IrcStalkTagSeparator { get; private set; }
        public string EmailRcTemplate { get; private set; }
        public string EmailStalkTemplate { get; private set; }
        public string DateFormat { get; private set; }
        public EmailConfiguration EmailConfiguration { get; private set; }
    }
}