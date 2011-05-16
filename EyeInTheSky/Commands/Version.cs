using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Version : GenericCommand
    {
        public Version()
        {
            this.requiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            string bindir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            DirectoryInfo di = new DirectoryInfo(bindir);
            di = di.Parent; //bin
            di = di.Parent; //eyeinthesky
            di = di.Parent; //eyeinthesky
            di = di.GetDirectories(".git")[0]; // .git


            if (GlobalFunctions.realArrayLength(tokens) < 1)
            {
                // look at .git/HEAD
                StreamReader sr = new StreamReader(di.FullName + "/HEAD");
                // get commit id from it
                string head = sr.ReadLine();
                sr.Close();
                if (head.StartsWith("ref:"))
                {
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Current " + head);
                }
                else
                {
                    // look in .git/info/refs
                    sr = new StreamReader(di.FullName + "/info/refs");

                    // find first line which has the commit id
                    // split line at whitespace - second part has the ref name ("refs/tags/release-1.2")
                    sr.Close();
                }

                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);
            if (mode == "update")
            {
            }
            if (mode == "switch")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string tag = GlobalFunctions.popFromFront(ref tokens);

            }
            if (mode == "list")
            {
            }
        }

        #endregion
    }
}
