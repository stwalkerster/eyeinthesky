using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Quick : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void Execute(IUser source, string destination, string[] tokens)
        {

        // =quick token type value

            if (tokens.Length < 3)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string name = GlobalFunctions.popFromFront(ref tokens);
            string type = GlobalFunctions.popFromFront(ref tokens);
            string regex = string.Join(" ", tokens);

            var s = new ComplexStalk(name);

            this.StalkConfig.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    UserStalkNode usn = new UserStalkNode();
                    usn.setMatchExpression(regex);
                    s.setSearchTree(usn, true);
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + usn);
                    break;
                case "page":
                    PageStalkNode psn = new PageStalkNode();
                    psn.setMatchExpression(regex);
                    s.setSearchTree(psn, true);
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + psn);
                    break;
                case "summary":
                    SummaryStalkNode ssn = new SummaryStalkNode();
                    ssn.setMatchExpression(regex);
                    s.setSearchTree(ssn, true);
                    this.Client.SendMessage(destination,
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
                        this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + node);


                    }
                    catch (XmlException)
                    {
                        this.Client.SendNotice(source.Nickname, "XML Error.");
                    }
                    break;
                default:
                    this.Client.SendNotice(source.Nickname, "Unknown stalk type!");
                    return;
            }

            s.enabled = true;
           
            this.StalkConfig.Save();
        }

        #endregion
    }
}
