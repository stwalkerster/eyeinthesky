using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    class Quick : GenericCommand
    {

        public Quick()
        {
            this.requiredAccessLevel = User.UserRights.Advanced;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {

        // =quick token type value

            if (tokens.Length < 3)
            {
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string name = GlobalFunctions.popFromFront(ref tokens);
            string type = GlobalFunctions.popFromFront(ref tokens);
            string regex = string.Join(" ", tokens);

            ComplexStalk s = new ComplexStalk(name);

            EyeInTheSkyBot.config.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    UserStalkNode usn = new UserStalkNode();
                    usn.setMatchExpression(regex);
                    s.setSearchTree(usn, true);
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + usn);
                    break;
                case "page":
                    PageStalkNode psn = new PageStalkNode();
                    psn.setMatchExpression(regex);
                    s.setSearchTree(psn, true);
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + psn);
                    break;
                case "summary":
                    SummaryStalkNode ssn = new SummaryStalkNode();
                    ssn.setMatchExpression(regex);
                    s.setSearchTree(ssn, true);
                    EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
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
                               "Set " + type + " for new stalk " + name +
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

            s.enabled = true;
           
            EyeInTheSkyBot.config.save();
        }

        #endregion
    }
}
