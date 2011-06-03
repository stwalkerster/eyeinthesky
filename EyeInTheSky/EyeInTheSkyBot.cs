using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    public class EyeInTheSkyBot
    {
        public static IAL irc_freenode, irc_wikimedia;
        public static Configuration config;
        public static void Main()
        {
            config = new Configuration("EyeInTheSky.config");

            string freenodepassword = config["nickservpassword"];

            // set up freenode connection

            irc_freenode = new IAL("chat.freenode.net", 8001, config["nickname", "EyeInTheSkyBot"], freenodepassword,
                                   "eyeinthesky", "Eye In The Sky", "NickServ");
           
            irc_freenode.NickServRegistrationSucceededEvent+=irc_freenode_connectionRegistrationSucceededEvent;
            irc_freenode.threadFatalError += irc_threadFatalError;
            irc_freenode.privmsgEvent += irc_freenode_privmsgEvent;

            // set up wikimedia connection

            irc_wikimedia = new IAL("irc.wikimedia.org", 6667, config["nickname", "EyeInTheSkyBot"], "", "eyeinthesky", "Eye In The Sky", "");
            irc_wikimedia.logEvents = bool.Parse(config["wikimediaIrcLog", "false"]);
            irc_wikimedia.connectionRegistrationSucceededEvent += irc_wikimedia_connectionRegistrationSucceededEvent;
            irc_wikimedia.threadFatalError += irc_threadFatalError;

            if ((!irc_freenode.connect()) || (!irc_wikimedia.connect()))
            {
                irc_freenode.stop();
                irc_wikimedia.stop();
                return;
            }
        }

        static void irc_freenode_privmsgEvent(User source, string destination, string message)
        {
           if(message.ToCharArray()[0].ToString() != config["commandtrigger", "="])
                return;

            string[] tokens = message.Split(' ');
            string command = GlobalFunctions.popFromFront(ref tokens);
            GenericCommand cmd = GenericCommand.create(command);
            if (cmd != null)
                cmd.run(source, destination, tokens);
            else
                irc_freenode.ircNotice(source.nickname, "Command not found.");
        }

        static void irc_threadFatalError(object sender, EventArgs e)
        {
            irc_freenode.stop();
            irc_wikimedia.stop();
        }

        static void irc_wikimedia_connectionRegistrationSucceededEvent()
        {
            irc_wikimedia.ircJoin(config["rcchannel", "#en.wikipedia"]);
        }

        static void irc_freenode_connectionRegistrationSucceededEvent()
        {
            irc_freenode.ircJoin(config["defaultchannel","##eyeinthesky"]);
            irc_wikimedia.privmsgEvent += irc_wikimedia_privmsgEvent;
        }

        static void irc_wikimedia_privmsgEvent(User source, string destination, string message)
        {
            RecentChange rcitem = RecentChange.parse(message);
            Stalk s = config.Stalks.search(rcitem);
            if(s==null) return;

            irc_freenode.ircPrivmsg(config["defaultchannel", "##eyeinthesky"], string.Format(
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