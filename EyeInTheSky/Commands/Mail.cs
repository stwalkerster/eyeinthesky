﻿using System;
using System.Net.Mail;
using System.Text;

namespace EyeInTheSky.Commands
{
    class Mail : GenericCommand
    {
        public Mail()
        {
            RequiredAccessLevel = User.UserRights.Developer;
        }

        #region Overrides of GenericCommand

        protected override void execute(User source, string destination, string[] tokens)
        {
            StalkLogItem[] sliList = EyeInTheSkyBot.Config.RetrieveStalkLog();
            var sb = new StringBuilder();
            sb.Append("This is the current stalk log from seen from the eye in the sky.\r\n");
            foreach (StalkLogItem item in sliList)
            {
                sb.Append("* " + item + "\r\n");
            }

            sb.Append("\r\nThis message was sent at " + DateTime.Now + " by " + source + ".\r\n\r\nThe Eye in the Sky");

            var mail = new MailMessage
                {
                    From = new MailAddress("eyeinthesky@eyeinthesky.im"),
                    Subject = "EyeInTheSky stalk log",
                    Body = sb.ToString()
                };
            mail.To.Add("simon@stwalkerster.co.uk");

            var client = new SmtpClient("mail.srv.stwalkerster.net");
            
            client.Send(mail);
            EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                                   "Sent stalk log via email to simon@stwalkerster.co.uk.");
            EyeInTheSkyBot.IrcFreenode.ircPrivmsg(destination,
                                       "Stalk log has been cleared.");

        }

        #endregion
    }
}
