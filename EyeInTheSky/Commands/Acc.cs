using System;
using System.Collections.Generic;
using System.Linq;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    using EyeInTheSky.Model;
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

            var id = tokenList.PopFromFront();
            var user = tokenList.Implode();

            var s = new ComplexStalk( "acc" + id );

            var or = new OrNode();

            var uor = new OrNode();
            var usn = new UserStalkNode();
            usn.SetMatchExpression(user);

            var psn = new PageStalkNode();
            psn.SetMatchExpression(user);

            uor.LeftChildNode = usn;
            uor.RightChildNode = psn;

            var upor = new OrNode();

            var upsn = new PageStalkNode();
            upsn.SetMatchExpression("User:" + user);

            var utpsn = new PageStalkNode();
            utpsn.SetMatchExpression("User talk:" + user);

            upor.LeftChildNode = upsn;
            upor.RightChildNode = utpsn;

            var ssn = new SummaryStalkNode();
            ssn.SetMatchExpression(user);
            
            var or2 = new OrNode();
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
