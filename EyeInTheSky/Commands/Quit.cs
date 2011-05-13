using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Quit : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            EyeInTheSkyBot.irc_wikimedia.ircQuit();
            EyeInTheSkyBot.irc_freenode.ircQuit();

            EyeInTheSkyBot.irc_wikimedia.stop();
            EyeInTheSkyBot.irc_freenode.stop();
        }

        #endregion
    }
}
