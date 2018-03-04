namespace EyeInTheSky.Services
{
    using System.Linq;
    using Castle.Core.Logging;
    using Stwalkerster.IrcClient.Events;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class RecentChangeHandler
    {
        private readonly AppConfiguration configuration;
        private readonly ILogger logger;
        private readonly StalkConfiguration stalkConfig;

        public RecentChangeHandler(AppConfiguration configuration, ILogger logger, StalkConfiguration stalkConfig)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.stalkConfig = stalkConfig;
        }

        public void OnReceivedMessage(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Command != "PRIVMSG")
            {
                return;
            }

            var parameters = e.Message.Parameters.ToList();
            string destination = parameters[0];
            string message = parameters[1];
            var source = IrcUser.FromPrefix(e.Message.Prefix);

            RecentChange rcitem = RecentChange.parse(message);
            
            Stalk s = this.stalkConfig.Stalks.search(rcitem);
            
            if (s == null)
            {
                return;
            }

            this.logger.InfoFormat("Seen stalked change for stalk {0}", s.Flag);
            
            ((IIrcClient) sender).SendMessage(
                this.configuration.FreenodeChannel,
                string.Format(
                    IrcColours.colorChar + "[{0}] Stalked edit {1} to page \"{2}\" by [[User:{3}]], summary: {4}",
                    IrcColours.colouredText(IrcColours.Colours.red, IrcColours.boldText(s.Flag)),
                    rcitem.Url,
                    IrcColours.colouredText(IrcColours.Colours.green, rcitem.Page),
                    rcitem.User,
                    IrcColours.colouredText(IrcColours.Colours.orange, rcitem.EditSummary)
                ));
        }
    }
}