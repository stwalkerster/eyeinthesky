using System;
using System.Collections.Generic;
using System.Linq;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using Stwalkerster.IrcClient.Extensions;
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Acc : GenericCommand
    {
        protected override void Execute(IUser source, string destination, IEnumerable<string> tokens)
        {
            var tokenList = tokens.ToList();
            
            if (tokenList.Count < 2)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string id = tokenList.PopFromFront();
            string user = tokenList.Implode();

            var s = new ComplexStalk( "acc" + id );

            OrNode or = new OrNode();

            OrNode uor = new OrNode();
            UserStalkNode usn = new UserStalkNode();
            usn.setMatchExpression(user);

            PageStalkNode psn = new PageStalkNode();
            psn.setMatchExpression(user);

            uor.LeftChildNode = usn;
            uor.RightChildNode = psn;

            OrNode upor = new OrNode();

            PageStalkNode upsn = new PageStalkNode();
            upsn.setMatchExpression("User:" + user);

            PageStalkNode utpsn = new PageStalkNode();
            utpsn.setMatchExpression("User talk:" + user);

            upor.LeftChildNode = upsn;
            upor.RightChildNode = utpsn;

            SummaryStalkNode ssn = new SummaryStalkNode();
            ssn.setMatchExpression(user);
            
            OrNode or2 = new OrNode();
            or2.LeftChildNode = uor;
            or2.RightChildNode = upor;
            
            or.LeftChildNode = or2;
            or.RightChildNode = ssn;
            s.MailEnabled = true;
            s.Description = "ACC " + id + ": " + user;
            s.SearchTree = or;
            s.IsEnabled = true;

            s.ExpiryTime = DateTime.Now.AddMonths(3);
            
            this.StalkConfig.Stalks.Add("acc" + id, s);
            this.StalkConfig.Save();

            this.Client.SendMessage(destination,
                "Set new stalk " + s.Flag +
                " with CSL value: " + or);
        }
    }
}
