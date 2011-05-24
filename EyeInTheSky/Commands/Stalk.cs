using System;
using System.Collections.Generic;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    class Stalk : GenericCommand
    {
        public Stalk()
        {
            this.requiredAccessLevel = User.UserRights.Advanced;
        }

        protected override void execute(User source, string destination, string[] tokens)
        {
            if (tokens.Length < 1)
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);
            if(mode == "add")
            #region add
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                ComplexStalk s = new ComplexStalk(tokens[0]);
                
                EyeInTheSkyBot.config.Stalks.Add(tokens[0],s);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Added stalk " + tokens[0]);
            }
            #endregion
            if (mode == "del")
            #region del
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                EyeInTheSkyBot.config.Stalks.Remove(tokens[0]);
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination, "Deleted stalk " + tokens[0]);
            }
            #endregion
            if (mode == "set")
            #region set
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);


                ComplexStalk s = EyeInTheSkyBot.config.Stalks[stalk];

                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string type = GlobalFunctions.popFromFront(ref tokens);

                    string regex = string.Join(" ", tokens);

                    switch (type)
                    {
                        case "user":
                            UserStalkNode usn = new UserStalkNode();
                            usn.setMatchExpression(regex);
                            s.setSearchTree(usn, true);
                            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + usn);
                            break;
                        case "page":
                            PageStalkNode psn = new PageStalkNode();
                            psn.setMatchExpression(regex);
                            s.setSearchTree(psn, true);
                            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + psn);
                            break;
                        case "summary":
                            SummaryStalkNode ssn = new SummaryStalkNode();
                            ssn.setMatchExpression(regex);
                            s.setSearchTree(ssn, true);
                            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + ssn);
                            break;
                        case "xml":
                            string xmlfragment = string.Join(" ", tokens);
                            try
                            {
                                XmlDocument xd = new XmlDocument();
                                xd.LoadXml(xmlfragment);


                                StalkNode node = StalkNode.newFromXmlFragment(xd.FirstChild);
                                s.setSearchTree(node, true);
                                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + node);


                            }
                            catch (XmlException)
                            {
                                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "XML Error.");
                            }
                            break;
                        default:
                            EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Unknown stalk type!");
                            return;
                    }
            }
            #endregion
            if (mode == "list")
            #region list
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Stalk list:");
                foreach (KeyValuePair<string, ComplexStalk> kvp in EyeInTheSkyBot.config.Stalks)
                {
                        EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname,
                                                              kvp.Value.ToString());

                }
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "End of stalk list.");
            }
            #endregion
            if(mode == "mail")
            #region mail
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                bool mail = bool.Parse(tokens[1]);
                EyeInTheSkyBot.config.Stalks[tokens[0]].mail = mail;
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                       "Set mail attribute on stalk " + tokens[0] + " to " + mail);
            }
            #endregion
            if(mode == "description")
            #region description
            {
                if (tokens.Length < 1)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }

                string stalk = GlobalFunctions.popFromFront(ref tokens);
                string descr = string.Join(" ", tokens);

                EyeInTheSkyBot.config.Stalks[stalk].Description = descr;
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Set description attribute on stalk " + stalk + " to " + descr);

            }
            #endregion
            if(mode == "expiry")
            #region expiry
            {
                if (tokens.Length < 2)
                {
                    EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);
                string date = string.Join(" ", tokens);

                DateTime expiryTime = DateTime.Parse(date);
                EyeInTheSkyBot.config.Stalks[stalk].expiryTime = expiryTime;
                EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                       "Set expiry attribute on stalk " + stalk + " to " + expiryTime);


            }
            #endregion


            EyeInTheSkyBot.config.save();
        }
        
    }
}
