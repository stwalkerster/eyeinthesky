using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            if (GlobalFunctions.realArrayLength(tokens) < 1)
            {
                Process p = new Process
                                {
                                    StartInfo =
                                        {
                                            UseShellExecute = false,
                                            RedirectStandardOutput = true,
                                            FileName = "git",
                                            Arguments = "describe"
                                        }
                                };
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                
                p.WaitForExit();

                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, output);
                return;
            }
        }

        #endregion
    }
}
