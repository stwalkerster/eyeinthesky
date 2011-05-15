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
                EyeInTheSkyBot.config.Stalks.Add(tokens[0],new SimpleStalk(tokens[0]));
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

                EyeInTheSky.Stalk s = EyeInTheSkyBot.config.Stalks[stalk];

                if (s is SimpleStalk)
                {
                    SimpleStalk ss = (SimpleStalk) s;

                    switch (type)
                    {
                        case "user":
                            ss.setUserSearch(regex);
                            break;
                        case "page":
                            ss.setPageSearch(regex);
                            break;
                        case "summary":
                            ss.setSummarySearch(regex);
                            break;
                    }

                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                           "Set " + type + " stalk for stalk " + stalk + " with value: " +
                                                           regex);
                }
                else
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,
                                       "The specified stalk is not a simple stalk.");
                }
            }
            if (mode == "list")
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Stalk list:");
                foreach (KeyValuePair<string, EyeInTheSky.Stalk> kvp in EyeInTheSkyBot.config.Stalks)
                {
                    if (kvp.Value is SimpleStalk)
                    {
                        SimpleStalk ss = (SimpleStalk) kvp.Value;
                        EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,
                                                              "Flag: " + kvp.Key + ", Type:" +
                                                              (ss.HasUserSearch ? " USER" : "") +
                                                              (ss.HasPageSearch ? " PAGE" : "") +
                                                              (ss.HasSummarySearch ? " SUMMARY" : ""));
                    }
                    else
                    {
                        EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,
                                                              kvp.Value.ToString());

                    }
                }
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "End of stalk list.");
            }
            EyeInTheSkyBot.config.save();
        }

        #endregion
    }
}
