using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Stalk : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            if(tokens.Length<2)
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,"Not enough params!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);

            if(mode == "add")
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Not enough params!");
                    return;
                }
                string type = GlobalFunctions.popFromFront(ref tokens);
                




            }
            if(mode == "del")
            {
                string flag = GlobalFunctions.popFromFront(ref tokens);
            }
        }

        #endregion
    }
}
