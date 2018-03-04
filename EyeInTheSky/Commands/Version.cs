using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EyeInTheSky.Commands
{
    using System.Linq;
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Version : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void Execute(IUser source, string destination, string[] tokens)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            if (directoryName != null)
            {
                string bindir = directoryName.Substring(6);
            
                if (tokens.Count(x => !string.IsNullOrEmpty(x)) < 1)
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


                    this.Client.SendMessage(destination, error);
                    this.Client.SendMessage(destination, "Version: " + output);
                
                }
            }
        }

        #endregion
    }
}
