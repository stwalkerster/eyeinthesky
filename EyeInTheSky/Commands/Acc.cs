using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using Stwalkerster.IrcClient.Model.Interfaces;

    class Acc : GenericCommand
    {
        protected override void Execute(IUser source, string destination, string[] tokens)
        {
            if (tokens.Length < 2)
            {
                this.Client.SendNotice(source.Nickname, "More params pls!");
                return;
            }

            string id = GlobalFunctions.popFromFront(ref tokens);
            string user = string.Join(" ", tokens);

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
            s.immediatemail = true;
            s.Description = "ACC " + id + ": " + user;
            s.mail = false;
            s.setSearchTree(or, true);
            s.enabled = true;

            s.expiryTime = DateTime.Now.AddMonths(3);
            
            this.StalkConfig.Stalks.Add("acc" + id, s);
            this.StalkConfig.Save();

            this.Client.SendMessage(destination,
                "Set new stalk " + s.Flag +
                " with CSL value: " + or);
        }
    }
}
