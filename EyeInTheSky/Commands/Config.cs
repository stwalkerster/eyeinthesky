using System;

namespace EyeInTheSky.Commands
{
    class Config : GenericCommand
    {
        public Config()
        {
            RequiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            if (tokens.Length < 1)
            {
                EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);

            if (mode == "get")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                try
                {
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                           tokens[0] + " == " + EyeInTheSkyBot.Config[tokens[0]]);

                }
                catch (ArgumentOutOfRangeException)
                {
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                           tokens[0] + " is non-existant");

                }
            }
            if (mode == "set")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                string setting = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.Config[setting] = string.Join(" ", tokens);
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                       setting + " = " + string.Join(" ", tokens));

            }
            if (mode == "del")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                string setting = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.Config.delete(setting);
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                       setting + " deleted");

            }
            if(mode == "save")
            {
                EyeInTheSkyBot.Config.save();
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                       "Configuration saved");
            }
            if (mode == "rehash")
            {
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                      EyeInTheSkyBot.Config.rehash()
                                                          ? "Configuration reloaded"
                                                          : "Configuration failed to reload.");
            }
        }

        #endregion
    }
}
