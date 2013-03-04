using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EyeInTheSky.StalkNodes;

namespace EyeInTheSky.Commands
{
    class Acc : GenericCommand
    {
        public Acc()
        {
            RequiredAccessLevel = User.UserRights.Advanced;
        }

        protected override void execute(User source, string destination, string[] tokens)
        {

        // =acc id user

            if (tokens.Length < 2)
            {
                EyeInTheSkyBot.IrcFreenode.ircNotice(source.nickname, "More params pls!");
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


            or.LeftChildNode = uor;
            or.RightChildNode = upor;
            s.immediatemail = true;
            s.Description = "ACC " + id + ": " + user;
            s.mail = false;
            s.setSearchTree(or, true);
            s.enabled = true;

            EyeInTheSkyBot.Config.Stalks.Add("acc" + id, s);
            EyeInTheSkyBot.Config.save();

            EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                "Set new stalk " + s.Flag +
                " with CSL value: " + or);
        }
    }
}
