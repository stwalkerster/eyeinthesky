namespace EyeInTheSky
{
    using System;

    public class AppConfiguration
    {
        public AppConfiguration(string freenodeChannel, string wikimediaChannel, string commandPrefix, string stalkConfigFile)
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

            this.FreenodeChannel = freenodeChannel;
            this.WikimediaChannel = wikimediaChannel;
            this.CommandPrefix = commandPrefix;
            this.StalkConfigFile = stalkConfigFile;
        }

        public string FreenodeChannel { get; private set; }
        public string WikimediaChannel { get; private set; }
        public string CommandPrefix { get; private set; }
        public string StalkConfigFile { get; private set; }
    }
}