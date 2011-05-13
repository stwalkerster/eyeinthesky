using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EyeInTheSky
{
    public class AccessListEntry
    {
        private string mask;
        private User.UserRights accesslevel;

        public AccessListEntry(string mask, User.UserRights accessLevel)
        {
            this.mask = mask;
            this.accesslevel = accessLevel;
        }

        public string HostnameMask
        {
            get { return mask; }
        }

        public User.UserRights AccessLevel
        {
            get { return accesslevel; }
        }
    }
}
