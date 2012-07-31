using System;
using System.Collections.Generic;

namespace EyeInTheSky.Commands
{
    class Access : GenericCommand
    {
        public Access()
        {
            RequiredAccessLevel = User.UserRights.Superuser;
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
            if (mode == "add")
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string hostmask = GlobalFunctions.popFromFront(ref tokens);
                string accesslevel = GlobalFunctions.popFromFront(ref tokens);
                var level = (User.UserRights)Enum.Parse(typeof(User.UserRights), accesslevel);

                if(source.accessLevel != User.UserRights.Developer && level >= source.accessLevel)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "Access denied.");
                    return;
                }

                var ale = new AccessListEntry(hostmask, level);
                EyeInTheSkyBot.Config.accessList.Add(hostmask, ale);
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination, "Done.");
            }
            if (mode == "del")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string hostmask = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.Config.accessList.Remove(hostmask);
                EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination, "Done.");
            }
            if (mode == "list")
            {
                EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "Access list:");
                foreach (KeyValuePair<string, AccessListEntry> accessListEntry in EyeInTheSkyBot.Config.accessList)
                {
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname,
                                                          accessListEntry.Value.HostnameMask + " = " +
                                                          accessListEntry.Value.AccessLevel);
                }
                EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "End of access list.");
            }
            EyeInTheSkyBot.Config.save();
        }

        #endregion
    }
}
