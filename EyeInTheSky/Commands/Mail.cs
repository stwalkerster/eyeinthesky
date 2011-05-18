using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Clear : GenericCommand
    {
        public Clear()
        {
            this.requiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            StalkLogItem[] sliList = EyeInTheSkyBot.config.RetrieveStalkLog();

            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Stalk log has been cleared.");

        }

        #endregion
    }
}
