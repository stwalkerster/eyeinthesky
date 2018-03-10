using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model;
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

            var name = tokenList.PopFromFront();
            var type = tokenList.PopFromFront();
            var stalkTarget = tokenList.Implode();

            var s = new ComplexStalk(name);

            if (this.StalkConfig.Stalks.ContainsKey(name))
            {
                this.Client.SendNotice(source.Nickname, "This stalk already exists!");
                return;
            }

            this.StalkConfig.Stalks.Add(name, s);

            switch (type)
            {
                case "user":
                    var usn = new UserStalkNode();
                    usn.SetMatchExpression(stalkTarget);
                    s.SearchTree = usn;
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + usn);
                    break;
                case "page":
                    var psn = new PageStalkNode();
                    psn.SetMatchExpression(stalkTarget);
                    s.SearchTree = psn;
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + psn);
                    break;
                case "summary":
                    var ssn = new SummaryStalkNode();
                    ssn.SetMatchExpression(stalkTarget);
                    s.SearchTree = ssn;
                    this.Client.SendMessage(destination,
                               "Set " + type + " for new stalk " + name +
                               " with CSL value: " + ssn);
                    break;
                case "xml":
                    var xmlfragment = stalkTarget;
                    try
                    {
                        var xd = new XmlDocument();
                        xd.LoadXml(xmlfragment);


                        var node = StalkNode.NewFromXmlFragment(xd.FirstChild);
                        s.SearchTree = node;
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

            s.IsEnabled = true;
           
            this.StalkConfig.Save();
        }

        #endregion
    }
}
