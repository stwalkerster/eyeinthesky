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

            // get/set/update password
            FileInfo pwfileinfo = new FileInfo("freenode.password");
            if(pwfileinfo.Exists)
            {
                StreamReader pwreader = new StreamReader("freenode.password");
                config["nickserv password"] = pwreader.ReadLine();
                pwreader.Close();
                config.save();
                pwfileinfo.Delete();
            }

            string freenodepassword = config["nickserv password"];

            // set up freenode connection

            irc_freenode = new IAL("chat.freenode.net", 8001, "EyeInTheSkyBot", freenodepassword,
                                   "eyeinthesky", "Eye In The Sky", "NickServ");
           
            irc_freenode.NickServRegistrationSucceededEvent+=irc_freenode_connectionRegistrationSucceededEvent;
            irc_freenode.threadFatalError += irc_threadFatalError;
            irc_freenode.privmsgEvent += irc_freenode_privmsgEvent;

            // set up wikimedia connection

            irc_wikimedia = new IAL("irc.wikimedia.org", 6667, "EyeInTheSky", "", "eyeinthesky", "Eye In The Sky", "");
            irc_wikimedia.connectionRegistrationSucceededEvent += irc_wikimedia_connectionRegistrationSucceededEvent;
            irc_wikimedia.threadFatalError += irc_threadFatalError;

            if((!irc_freenode.connect()) || (!irc_wikimedia.connect()))
            {
                irc_freenode.stop();
                irc_wikimedia.stop();
            }

        }

        static void irc_freenode_privmsgEvent(User source, string destination, string message)
        {
            if (destination != "##eyeinthesky")
                return;

            if(message.ToCharArray()[0] != '=')
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
            irc_wikimedia.ircJoin("#en.wikipedia");
        }

        static void irc_freenode_connectionRegistrationSucceededEvent()
        {
            irc_freenode.ircJoin("##eyeinthesky");
        }
    }
}