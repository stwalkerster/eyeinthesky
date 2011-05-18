using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Mail : GenericCommand
    {
        public Mail()
        {
            this.requiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            StalkLogItem[] sliList = EyeInTheSkyBot.config.RetrieveStalkLog();
            StringBuilder sb = new StringBuilder();
            sb.Append("This is the current stalk log from seen from the eye in the sky.\r\n");
            foreach (StalkLogItem item in sliList)
            {
                sb.Append("* " + item + "\r\n");
            }

            sb.Append("\r\nThis message was sent at " + DateTime.Now + " by " + source + ".\r\n\r\nThe Eye in the Sky");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("eyeinthesky@helpmebot.org.uk");
            mail.To.Add("stwalkerster@helpmebot.org.uk");
            mail.Subject = "EyeInTheSky stalk log";
            mail.Body = sb.ToString();

            SmtpClient client = new SmtpClient();
            client.Send(mail);
            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                                   "Sent stalk log via email to stwalkerster@helpmebot.org.uk.");
            EyeInTheSkyBot.irc_freenode.ircPrivmsg(destination,
                                       "Stalk log has been cleared.");

        }

        #endregion
    }
}
