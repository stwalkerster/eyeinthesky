using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Config : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            if (tokens.Length < 1)
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);

            if (mode == "get")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                try
                {
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                           tokens[0] + " == " + EyeInTheSkyBot.config[tokens[0]]);

                }
                catch (ArgumentOutOfRangeException)
                {
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                           tokens[0] + " is non-existant");

                }
            }
            if (mode == "set")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                string setting = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.config[setting] = string.Join(" ", tokens);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                       setting + " = " + string.Join(" ", tokens));

            }
            if (mode == "del")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                string setting = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.config.delete(setting);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                       setting + " deleted");

            }
            if(mode == "save")
            {
                EyeInTheSkyBot.config.save();
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Configuration saved");
            }
            if (mode == "rehash")
            {
                EyeInTheSkyBot.config.rehash();
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Configuration reloaded");
            }
        }

        #endregion
    }
}
