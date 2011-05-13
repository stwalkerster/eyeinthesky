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
            EyeInTheSkyBot.irc_freenode.stop();
            EyeInTheSkyBot.irc_wikimedia.stop();
        }

        #endregion
    }
}
