namespace EyeInTheSky.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
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
            var user = this.botUserConfiguration.Items.FirstOrDefault(x => x.WebGuid == identifier);
            if (user == null)
            {
                return null;
            }

            return new UserIdentity(user);
        }


    }
}