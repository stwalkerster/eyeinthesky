using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using helpmebot6;

namespace EyeInTheSky
{
    public class EyeInTheSkyBot
    {
        public static IAL irc_freenode, irc_wikimedia;

        public static void Main()
        {
            // get password

            if(!new FileInfo("freenode.password").Exists)
            {
                new FileInfo("freenode.password").Create();
                return;
            }

            string freenodepassword = new StreamReader("freenode.password").ReadLine();

            // set up freenode connection

            irc_freenode = new IAL("chat.freenode.net", 8001, "EyeInTheSkyBot", freenodepassword,
                                   "eyeinthesky", "Eye In The Sky");
            irc_freenode.connectionRegistrationSucceededEvent += irc_freenode_connectionRegistrationSucceededEvent;
            irc_freenode.threadFatalError += irc_threadFatalError;
            irc_freenode.privmsgEvent += irc_freenode_privmsgEvent;

            // set up wikimedia connection

            irc_wikimedia = new IAL("irc.wikimedia.org", 6667, "EyeInTheSky", "", "eyeinthesky", "Eye In The Sky");
            irc_wikimedia.connectionRegistrationSucceededEvent += irc_wikimedia_connectionRegistrationSucceededEvent;
            irc_wikimedia.threadFatalError += irc_threadFatalError;
        }

        static void irc_freenode_privmsgEvent(User source, string destination, string message)
        {
            if (destination != "##eyeinthesky")
                return;

            if(message.ToCharArray()[0] != '=')
                return;

            string[] tokens = message.Split(' ');
            
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
