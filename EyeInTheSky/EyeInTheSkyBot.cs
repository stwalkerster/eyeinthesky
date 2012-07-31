using System;
using System.Globalization;

namespace EyeInTheSky
{
    public class EyeInTheSkyBot
    {
        public static IAL IrcFreenode, IrcWikimedia;
        public static Configuration Config;
        private static Nagios _nag;
        public static void Main()
        {
            Config = new Configuration("EyeInTheSky.config");

            string freenodepassword = Config["nickservpassword"];

            // set up freenode connection

            IrcFreenode = new IAL("chat.freenode.net", 8001, Config["nickname", "EyeInTheSkyBot"], freenodepassword,
                                   "eyeinthesky", "Eye In The Sky", "NickServ");
           
            IrcFreenode.NickServRegistrationSucceededEvent+=irc_freenode_connectionRegistrationSucceededEvent;
            IrcFreenode.threadFatalError += irc_threadFatalError;
            IrcFreenode.privmsgEvent += irc_freenode_privmsgEvent;

            // set up wikimedia connection

            IrcWikimedia = new IAL("irc.wikimedia.org", 6667, Config["nickname", "EyeInTheSkyBot"], "", "eyeinthesky", "Eye In The Sky", "");
            IrcWikimedia.logEvents = bool.Parse(Config["wikimediaIrcLog", "false"]);
            IrcWikimedia.connectionRegistrationSucceededEvent += irc_wikimedia_connectionRegistrationSucceededEvent;
            IrcWikimedia.threadFatalError += irc_threadFatalError;

            _nag = new Nagios();
            

            if ((!IrcFreenode.connect()) || (!IrcWikimedia.connect()))
            {
                IrcFreenode.stop();
                IrcWikimedia.stop();
                _nag.stop();
            }
        }

        static void irc_freenode_privmsgEvent(User source, string destination, string message)
        {
           if(message.ToCharArray()[0].ToString(CultureInfo.InvariantCulture) != Config["commandtrigger", "="])
                return;

            string[] tokens = message.Split(' ');
            string command = GlobalFunctions.popFromFront(ref tokens);
            GenericCommand cmd = GenericCommand.create(command);
            if (cmd != null)
                cmd.run(source, destination, tokens);
            else
                IrcFreenode.ircNotice(source.nickname, "Command not found.");
        }

        static void irc_threadFatalError(object sender, EventArgs e)
        {
            IrcFreenode.stop();
            IrcWikimedia.stop();
        }

        static void irc_wikimedia_connectionRegistrationSucceededEvent()
        {
            IrcWikimedia.ircJoin(Config["rcchannel", "#en.wikipedia"]);
        }

        static void irc_freenode_connectionRegistrationSucceededEvent()
        {
            IrcFreenode.ircJoin(Config["defaultchannel","##eyeinthesky"]);
            IrcWikimedia.privmsgEvent += irc_wikimedia_privmsgEvent;
        }

        static void irc_wikimedia_privmsgEvent(User source, string destination, string message)
        {
            RecentChange rcitem = RecentChange.parse(message);
            Stalk s = Config.Stalks.search(rcitem);
            if(s==null) return;

            IrcFreenode.ircPrivmsg(Config["defaultchannel", "##eyeinthesky"], string.Format(
               IrcColours.colorChar + "[{0}] Stalked edit {1} to page \"{2}\" by [[User:{3}]], summary: {4}",
                IrcColours.colouredText(IrcColours.Colours.red,IrcColours.boldText(s.Flag)),
                rcitem.Url, 
                IrcColours.colouredText(IrcColours.Colours.green, rcitem.Page), 
                rcitem.User,
                IrcColours.colouredText(IrcColours.Colours.orange, rcitem.EditSummary)
                                                         ));
        }
    }
}