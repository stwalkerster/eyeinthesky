using System;
using System.Collections.Generic;
using System.Linq;
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
            if (tokens.Length < 1)
            {
                // print current tag
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
