namespace EyeInTheSky.Web.Services
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Services.Interfaces;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;

    public class NancyUserMapper : IUserMapper
    {
        private readonly IBotUserConfiguration botUserConfiguration;

        public NancyUserMapper(IBotUserConfiguration botUserConfiguration)
        {
            this.botUserConfiguration = botUserConfiguration;
        }
        
        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            return new UserIdentity("webuser", new[] {"Admin"});
        }

        public class UserIdentity : IUserIdentity
        {
            public UserIdentity(string userName, IEnumerable<string> claims)
            {
                this.UserName = userName;
                this.Claims = claims;
            }

            public string UserName { get; private set; }
            public IEnumerable<string> Claims { get; private set; }
        }
    }
}