using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Quick : GenericCommand
    {
        #region Overrides of GenericCommand

        protected override void Execute(IUser source, string destination, IEnumerable<string> tokens)
        {
            var tokenList = tokens.ToList();

            if (tokenList.Count < 3)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string name = tokenList.PopFromFront();
            string type = tokenList.PopFromFront();
            string stalkTarget = tokenList.Implode();

            var s = new ComplexStalk(name);

            this.StalkConfig.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    UserStalkNode usn = new UserStalkNode();
                    usn.setMatchExpression(stalkTarget);
                    s.setSearchTree(usn, true);
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + usn);
                    break;
                case "page":
                    PageStalkNode psn = new PageStalkNode();
                    psn.setMatchExpression(stalkTarget);
                    s.setSearchTree(psn, true);
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + psn);
                    break;
                case "summary":
                    SummaryStalkNode ssn = new SummaryStalkNode();
                    ssn.setMatchExpression(stalkTarget);
                    s.setSearchTree(ssn, true);
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + ssn);
                    break;
                case "xml":
                    string xmlfragment = stalkTarget;
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
