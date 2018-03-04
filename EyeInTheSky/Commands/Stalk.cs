using System;
using System.Collections.Generic;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Stalk : GenericCommand
    {
        protected override void Execute(IUser source, string destination, string[] tokens)
        {
            if (tokens.Length < 1)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string mode = GlobalFunctions.popFromFront(ref tokens);
            if(mode == "add")
            #region add
            {
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                var s = new ComplexStalk(tokens[0]);
                
                this.StalkConfig.Stalks.Add(tokens[0],s);
                this.Client.SendMessage(destination, "Added stalk " + tokens[0]);
            }
            #endregion
            if (mode == "del")
            #region del
            {
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                this.StalkConfig.Stalks.Remove(tokens[0]);
                this.Client.SendMessage(destination, "Deleted stalk " + tokens[0]);
            }
            #endregion
            if (mode == "set")
            #region set
            {
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);


                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = GlobalFunctions.popFromFront(ref tokens);

                    string regex = string.Join(" ", tokens);

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
                            string xmlfragment = string.Join(" ", tokens);
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
                if (tokens.Length < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                if(tokens[1] == "immediate")
                {
                    if (tokens.Length < 3 )
                    {
                        this.Client.SendNotice(source.Nickname, "More params pls!");
                        return;
                    }

                    bool imail = bool.Parse(tokens[2]);
                    this.StalkConfig.Stalks[tokens[0]].immediatemail = imail;
                    this.Client.SendMessage(destination,
                                                           "Set immediatemail attribute on stalk " + tokens[0] + " to " + imail);
                }

                bool mail = bool.Parse(tokens[1]);
                this.StalkConfig.Stalks[tokens[0]].mail = mail;
                this.Client.SendMessage(destination,
                                                       "Set mail attribute on stalk " + tokens[0] + " to " + mail);
            }
            #endregion
            if(mode == "description")
            #region description
            {
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                string stalk = GlobalFunctions.popFromFront(ref tokens);
                string descr = string.Join(" ", tokens);

                this.StalkConfig.Stalks[stalk].Description = descr;
                this.Client.SendMessage(destination,
                                       "Set description attribute on stalk " + stalk + " to " + descr);

            }
            #endregion
            if(mode == "expiry")
            #region expiry
            {
                if (tokens.Length < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);
                string date = string.Join(" ", tokens);

                DateTime expiryTime = DateTime.Parse(date);
                this.StalkConfig.Stalks[stalk].expiryTime = expiryTime;
                this.Client.SendMessage(destination,
                                                       "Set expiry attribute on stalk " + stalk + " to " + expiryTime);


            }
            #endregion
            if (mode == "enabled")
            #region enabled
            {
                if (tokens.Length < 2)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }

                bool enabled = bool.Parse(tokens[1]);
                this.StalkConfig.Stalks[tokens[0]].enabled = enabled;
                this.Client.SendMessage(destination,
                                                       "Set enabled attribute on stalk " + tokens[0] + " to " + enabled);
            }
            #endregion
            if (mode == "and")
            #region and
            {
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);


                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = GlobalFunctions.popFromFront(ref tokens);

                string regex = string.Join(" ", tokens);

                var newroot = new AndNode {LeftChildNode = s.getSearchTree()};

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.setMatchExpression(regex);
                        newroot.RightChildNode = usn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.setMatchExpression(regex);
                        newroot.RightChildNode = psn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.setMatchExpression(regex);
                        newroot.RightChildNode = ssn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "xml":
                        string xmlfragment = string.Join(" ", tokens);
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
                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string stalk = GlobalFunctions.popFromFront(ref tokens);


                ComplexStalk s = this.StalkConfig.Stalks[stalk];

                if (tokens.Length < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = GlobalFunctions.popFromFront(ref tokens);

                string regex = string.Join(" ", tokens);

                var newroot = new OrNode { LeftChildNode = s.getSearchTree() };

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.setMatchExpression(regex);
                        newroot.RightChildNode = usn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.setMatchExpression(regex);
                        newroot.RightChildNode = psn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.setMatchExpression(regex);
                        newroot.RightChildNode = ssn;
                        s.setSearchTree(newroot, true);
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "xml":
                        string xmlfragment = string.Join(" ", tokens);
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
