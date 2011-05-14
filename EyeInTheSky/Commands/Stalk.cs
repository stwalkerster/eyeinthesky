using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Stalk : GenericCommand
    {
        public Stalk()
        {
            this.requiredAccessLevel = User.UserRights.Advanced;
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
            if(mode == "add")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                EyeInTheSkyBot.config.Stalks.Add(tokens[0],new EyeInTheSky.Stalk(tokens[0]));
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Added stalk " + tokens[0]);
            }
            if (mode == "del")
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                EyeInTheSkyBot.config.Stalks.Remove(tokens[0]);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Deleted stalk " + tokens[0]);
            }
            if (mode == "set")
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);
                string type = GlobalFunctions.popFromFront(ref tokens);
                string regex = string.Join(" ", tokens);

                switch (type)
                {
                    case "user":
                        EyeInTheSkyBot.config.Stalks[stalk].setUserSearch(regex);
                        break;
                    case "page":
                        EyeInTheSkyBot.config.Stalks[stalk].setPageSearch(regex);
                        break;
                    case "summary":
                        EyeInTheSkyBot.config.Stalks[stalk].setSummarySearch(regex);
                        break;
                }

                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                       "Set " + type + " stalk for stalk " + stalk + " with value: " +
                                                       regex);

            }
            if (mode == "list")
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Stalk list:");
                foreach (KeyValuePair<string, EyeInTheSky.Stalk> kvp in EyeInTheSkyBot.config.Stalks)
                {
                     EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Flag: " + kvp.Key + ", Type:"
                         + (kvp.Value.HasUserSearch ? " USER" : "")
                         + (kvp.Value.HasPageSearch ? " PAGE" : "")
                         + (kvp.Value.HasSummarySearch ? " SUMMARY" : "")
                         );
                }
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "End of stalk list.");
            }
            EyeInTheSkyBot.config.save();
        }

        #endregion
    }
}
