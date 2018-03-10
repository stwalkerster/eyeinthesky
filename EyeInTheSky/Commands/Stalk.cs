﻿using System;
using System.Collections.Generic;
using System.Xml;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using System.Linq;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
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
                
                IStalk s = this.StalkConfig.Stalks[stalk];

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
                            usn.SetMatchExpression(regex);
                            s.SearchTree = usn;
                            this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + usn);
                            break;
                        case "page":
                            var psn = new PageStalkNode();
                            psn.SetMatchExpression(regex);
                            s.SearchTree = psn;
                            this.Client.SendMessage(destination,
                                       "Set " + type + " for stalk " + stalk +
                                       " with CSL value: " + psn);
                            break;
                        case "summary":
                            var ssn = new SummaryStalkNode();
                            ssn.SetMatchExpression(regex);
                            s.SearchTree = ssn;
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


                                StalkNode node = StalkNode.NewFromXmlFragment(xd.FirstChild);
                                s.SearchTree = node;
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
                foreach (var kvp in this.StalkConfig.Stalks)
                {
                    this.Client.SendNotice(source.Nickname, kvp.Value.ToString());

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

                this.Client.SendMessage(destination, "Set immediatemail attribute on stalk " + stalkName + " to " + mail);
                this.StalkConfig.Stalks[stalkName].MailEnabled = mail;
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
                var stalk = tokenList.PopFromFront();
                var date = tokenList.Implode();

                var expiryTime = DateTime.Parse(date);
                this.StalkConfig.Stalks[stalk].ExpiryTime = expiryTime;
                this.Client.SendMessage(destination, "Set expiry attribute on stalk " + stalk + " to " + expiryTime);


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
                var enabled = bool.Parse(tokenList.PopFromFront());
                this.Client.SendMessage(destination, "Set enabled attribute on stalk " + stalkName + " to " + enabled);
                this.StalkConfig.Stalks[stalkName].IsEnabled = enabled;
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


                var s = this.StalkConfig.Stalks[stalk];

                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                string type = tokenList.PopFromFront();

                string stalkTarget = tokenList.Implode();

                var newroot = new AndNode {LeftChildNode = s.SearchTree};

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = usn;
                        s.SearchTree = newroot;
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = psn;
                        s.SearchTree = newroot;
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = ssn;
                        s.SearchTree = newroot;
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


                            StalkNode node = StalkNode.NewFromXmlFragment(xd.FirstChild);

                            newroot.RightChildNode = node;
                            s.SearchTree = newroot;
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


                var s = this.StalkConfig.Stalks[stalk];

                if (tokenList.Count < 1)
                {
                    this.Client.SendNotice(source.Nickname, "More params pls!");
                    return;
                }
                
                string type = tokenList.PopFromFront();

                string stalkTarget = tokenList.Implode();

                var newroot = new OrNode { LeftChildNode = s.SearchTree };

                switch (type)
                {
                    case "user":
                        var usn = new UserStalkNode();
                        usn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = usn;
                        s.SearchTree = newroot;
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "page":
                        var psn = new PageStalkNode();
                        psn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = psn;
                        s.SearchTree = newroot;
                        this.Client.SendMessage(destination,
                                   "Set " + type + " for stalk " + stalk +
                                   " with CSL value: " + newroot);
                        break;
                    case "summary":
                        var ssn = new SummaryStalkNode();
                        ssn.SetMatchExpression(stalkTarget);
                        newroot.RightChildNode = ssn;
                        s.SearchTree = newroot;
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


                            StalkNode node = StalkNode.NewFromXmlFragment(xd.FirstChild);

                            newroot.RightChildNode = node;
                            s.SearchTree = newroot;
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
