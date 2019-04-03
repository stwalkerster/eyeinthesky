namespace EyeInTheSky.Web.Misc
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using Nancy.Security;

    public class UserIdentity : IUserIdentity
    {
        private readonly IBotUser botUser;

        public UserIdentity(IBotUser botUser)
        {
            this.botUser = botUser;
        }

        public string UserName
        {
            get { return this.botUser.Identifier; }
        }

        public IEnumerable<string> Claims
        {
            get
            {
                return new List<string>();
            }
        }
    }
}