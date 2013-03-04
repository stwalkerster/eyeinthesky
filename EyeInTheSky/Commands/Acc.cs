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

            UserStalkNode usn = new UserStalkNode();
            usn.setMatchExpression(user);

            PageStalkNode psn = new PageStalkNode();
            psn.setMatchExpression(user);

            or.LeftChildNode = usn;
            or.RightChildNode = psn;
            s.immediatemail = true;
            s.Description = "ACC " + id + ": " + user;
            s.mail = false;
            s.setSearchTree(or, true);
            s.enabled = true;

            EyeInTheSkyBot.Config.Stalks.Add("acc" + id, s);
            EyeInTheSkyBot.Config.save();
        }
    }
}
