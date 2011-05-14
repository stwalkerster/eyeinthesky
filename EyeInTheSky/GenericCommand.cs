using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    abstract class GenericCommand
    {
        protected User.UserRights requiredAccessLevel = User.UserRights.Developer;

        public static GenericCommand create(string command)
        {
            command = command.Substring(1);
            string trueCommand = command.Substring(0, 1).ToUpper() + command.Substring(1).ToLower();
            trueCommand = "EyeInTheSky.Commands." + trueCommand;

            Type t = Type.GetType(trueCommand);
            if (t == null) return null;
            return (GenericCommand) Activator.CreateInstance(t);
        }

        public void run(User source, string destination, string[] tokens)
        {
            if (source.accessLevel < this.requiredAccessLevel)
                EyeInTheSkyBot.irc_freenode.ircNotice(source.nickname, "Access denied.");
            else
                execute(source, destination, tokens);
        }
        
        protected abstract void execute(User source, string destination, string[] tokens);
    }
}
