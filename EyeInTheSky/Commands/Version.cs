using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EyeInTheSky.Commands
{
    class Version : GenericCommand
    {
        public Version()
        {
            RequiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (directoryName != null)
            {
                string bindir = directoryName.Substring(6);
            
                if (GlobalFunctions.realArrayLength(tokens) < 1)
                {
                    var p = new Process
                        {
                            StartInfo =
                                {
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    FileName = "git",
                                    Arguments = "describe",
                                    WorkingDirectory = bindir
                                            
                                }
                        };
                    p.Start();

                    string output = p.StandardOutput.ReadToEnd();
                    string error = p.StandardError.ReadToEnd();
                
                    p.WaitForExit();


                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination, error);
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination, "Version: " + output);
                
                }
            }
        }

        #endregion
    }
}
