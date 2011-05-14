using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Access : GenericCommand
    {
        public Access()
        {
            this.requiredAccessLevel = User.UserRights.Superuser;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            if (tokens.Length < 1)
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);
            if (mode == "add")
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string hostmask = GlobalFunctions.popFromFront(ref tokens);
                string accesslevel = GlobalFunctions.popFromFront(ref tokens);
                User.UserRights level = (User.UserRights)Enum.Parse(typeof(User.UserRights), accesslevel);

                AccessListEntry ale = new AccessListEntry(hostmask, level);
                EyeInTheSkyBot.config.accessList.Add(hostmask, ale);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Done.");
            }
            if (mode == "del")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string hostmask = GlobalFunctions.popFromFront(ref tokens);
                EyeInTheSkyBot.config.accessList.Remove(hostmask);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Done.");
            }
            if (mode == "list")
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Access list:");
                foreach (KeyValuePair<string, AccessListEntry> accessListEntry in EyeInTheSkyBot.config.accessList)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,
                                                          accessListEntry.Value.HostnameMask + " = " +
                                                          accessListEntry.Value.AccessLevel);
                }
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "End of access list.");
            }
            EyeInTheSkyBot.config.save();
        }

        #endregion
    }
}
