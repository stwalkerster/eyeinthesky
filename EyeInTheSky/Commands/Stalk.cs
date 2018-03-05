using System;
using System.Collections.Generic;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using System.Linq;
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Stalk : GenericCommand
    {
        protected override void Execute(IUser source, string destination, IEnumerable<string> tokens)
        {
            var tokenList = tokens.ToList();
            
            if (tokenList.Count < 1)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string mode =tokenList.PopFromFront();
            if(mode == "add")
            #region add
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                var stalkName = tokenList.First();
                var s = new ComplexStalk(stalkName);
                
                this.StalkConfig.Stalks.Add(stalkName,s);
                this.Client.SendMessage(destination, "Added stalk " + stalkName);
            }
            #endregion
            if (mode == "del")
            #region del
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                
                var stalkName = tokenList.First();
                
                this.StalkConfig.Stalks.Remove(stalkName);
                this.Client.SendMessage(destination, "Deleted stalk " + stalkName);
            }
            #endregion
            if (mode == "set")
            #region set
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = tokenList.PopFromFront();

                if (!this.StalkConfig.Stalks.ContainsKey(stalk))
                {
                    this.Client.SendNotice(source.Nickname, "Can't find the stalk '" + stalk + "'!");
                    return;
                }
                
                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = tokenList.PopFromFront();

                    string regex = tokenList.Implode();

                    switch (type)
                    {
                        case "user":
                            var usn = new UserStalkNode();
                            usn.setMatchExpression(regex);
                            s.setSearchTree(usn, true);
                            this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + usn);
                            break;
                        case "page":
                            var psn = new PageStalkNode();
                            psn.setMatchExpression(regex);
                            s.setSearchTree(psn, true);
                            this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + psn);
                            break;
                        case "summary":
                            var ssn = new SummaryStalkNode();
                            ssn.setMatchExpression(regex);
                            s.setSearchTree(ssn, true);
                            this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + ssn);
                            break;
                        case "xml":
                            string xmlfragment = tokenList.Implode();
                            try
                            {
                                var xd = new XmlDocument();
                                xd.LoadXml(xmlfragment);


                                StalkNode node = StalkNode.newFromXmlFragment(xd.FirstChild);
                                s.setSearchTree(node, true);
                                this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
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
            }
            #endregion
            if (mode == "list")
            #region list
            {
                this.Client.SendNotice(source.Nickname, "Stalk list:");
                foreach (KeyValuePair<string, ComplexStalk> kvp in this.StalkConfig.Stalks)
                {
                        this.Client.SendNotice(source.Nickname,
                                                              kvp.Value.ToString());

                }
                this.Client.SendNotice(source.Nickname, "End of stalk list.");
            }
            #endregion
            if(mode == "mail")
            #region mail
            {
                if (tokenList.Count < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                var stalkName = tokenList.PopFromFront();

                bool mail;
                var possibleBoolean = tokenList.PopFromFront();
                if (!bool.TryParse(possibleBoolean, out mail))
                {
                    this.Client.SendNotice(source.Nickname, possibleBoolean + " is not a value of boolean I recognise. Try 'true', 'false' or ERR_FILE_NOT_FOUND.");
                    return;
                }

                this.StalkConfig.Stalks[stalkName].immediatemail = mail;
                this.Client.SendMessage(destination, "Set immediatemail attribute on stalk " + stalkName + " to " + mail);
            }
            #endregion
            if(mode == "description")
            #region description
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                string stalk = tokenList.PopFromFront();
                string descr = tokenList.Implode();

                this.StalkConfig.Stalks[stalk].Description = descr;
                this.Client.SendMessage(destination,
                                       "Set description attribute on stalk " + stalk + " to " + descr);

            }
            #endregion
            if(mode == "expiry")
            #region expiry
            {
                if (tokenList.Count < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = tokenList.PopFromFront();
                string date = tokenList.Implode();

                DateTime expiryTime = DateTime.Parse(date);
                this.StalkConfig.Stalks[stalk].expiryTime = expiryTime;
                this.Client.SendMessage(destination,
                                                       "Set expiry attribute on stalk " + stalk + " to " + expiryTime);


            }
            #endregion
            if (mode == "enabled")
            #region enabled
            {
                if (tokenList.Count < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                var stalkName = tokenList.PopFromFront();
                bool enabled = bool.Parse(tokenList.PopFromFront());
                this.StalkConfig.Stalks[stalkName].enabled = enabled;
                this.Client.SendMessage(destination, "Set enabled attribute on stalk " + stalkName + " to " + enabled);
            }
            #endregion
            if (mode == "and")
            #region and
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = tokenList.PopFromFront();


                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = tokenList.PopFromFront();

                string stalkTarget = tokenList.Implode();

                var newroot = new AndNode {LeftChildNode = s.getSearchTree()};

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = usn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = psn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = ssn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "xml":
                        string xmlfragment = stalkTarget;
                        try
                        {
                            var xd = new XmlDocument();
                            xd.LoadXml(xmlfragment);


                            StalkNode node = StalkNode.newFromXmlFragment(xd.FirstChild);

                            newroot.RightChildNode = node;
                            s.setSearchTree(newroot, true);
                            this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);


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
            }
            #endregion
            if (mode == "or")
            #region or
            {
                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = tokenList.PopFromFront();


                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                
                string type = tokenList.PopFromFront();

                string stalkTarget = tokenList.Implode();

                var newroot = new OrNode { LeftChildNode = s.getSearchTree() };

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = usn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = psn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.setMatchExpression(stalkTarget);
                        newroot.RightChildNode = ssn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "xml":
                        string xmlfragment = stalkTarget;
                        try
                        {
                            var xd = new XmlDocument();
                            xd.LoadXml(xmlfragment);


                            StalkNode node = StalkNode.newFromXmlFragment(xd.FirstChild);

                            newroot.RightChildNode = node;
                            s.setSearchTree(newroot, true);
                            this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);


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
            }
            #endregion
            this.StalkConfig.Save();
        }
        
    }
}
