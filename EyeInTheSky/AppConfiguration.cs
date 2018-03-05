namespace EyeInTheSky
{
    using System;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class AppConfiguration
    {
        public AppConfiguration(string freenodeChannel,
            string wikimediaChannel,
            string commandPrefix,
            string stalkConfigFile,
            string rcUser)
        {
            if (freenodeChannel == null)
            {
                throw new ArgumentNullException("freenodeChannel");
            }

            if (string.IsNullOrWhiteSpace(freenodeChannel))
            {
                throw new ArgumentOutOfRangeException("freenodeChannel");
            }

            if (wikimediaChannel == null)
            {
                throw new ArgumentNullException("wikimediaChannel");
            }

            if (string.IsNullOrWhiteSpace(wikimediaChannel))
            {
                throw new ArgumentOutOfRangeException("wikimediaChannel");
            }

            if (commandPrefix == null)
            {
                throw new ArgumentNullException("commandPrefix");
            }

            if (commandPrefix.Length != 1)
            {
                throw new ArgumentOutOfRangeException("commandPrefix");
            }

            if (stalkConfigFile == null)
            {
                throw new ArgumentNullException("stalkConfigFile");
            }

            if (rcUser == null)
            {
                throw new ArgumentNullException("rcUser");
            }

            this.FreenodeChannel = freenodeChannel;
            this.WikimediaChannel = wikimediaChannel;
            this.CommandPrefix = commandPrefix;
            this.StalkConfigFile = stalkConfigFile;
            this.RcUser = IrcUser.FromPrefix(rcUser);
        }

        public string FreenodeChannel { get; private set; }
        public string WikimediaChannel { get; private set; }
        public string CommandPrefix { get; private set; }
        public string StalkConfigFile { get; private set; }
        public IUser RcUser { get; private set; }
    }
}