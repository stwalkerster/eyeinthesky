using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    class Quick : GenericCommand
    {

        public Quick()
        {
            RequiredAccessLevel = User.UserRights.Advanced;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {

        // =quick token type value

            if (tokens.Length < 3)
            {
                EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
                return;
            }

            string name = GlobalFunctions.popFromFront(ref tokens);
            string type = GlobalFunctions.popFromFront(ref tokens);
            string regex = string.Join(" ", tokens);

            var s = new ComplexStalk(name);

            EyeInTheSkyBot.Config.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    UserStalkNode usn = new UserStalkNode();
                    usn.setMatchExpression(regex);
                    s.setSearchTree(usn, true);
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + usn);
                    break;
                case "page":
                    PageStalkNode psn = new PageStalkNode();
                    psn.setMatchExpression(regex);
                    s.setSearchTree(psn, true);
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + psn);
                    break;
                case "summary":
                    SummaryStalkNode ssn = new SummaryStalkNode();
                    ssn.setMatchExpression(regex);
                    s.setSearchTree(ssn, true);
                    EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
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
                        EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + node);


                    }
                    catch (XmlException)
                    {
                        EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "XML Error.");
                    }
                    break;
                default:
                    EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "Unknown stalk type!");
                    return;
            }

            s.enabled = true;
           
            EyeInTheSkyBot.Config.save();
        }

        #endregion
    }
}
