namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class BasicFlagService : IFlagService
    {
        private readonly IAppConfiguration appConfiguration;

        public BasicFlagService(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public bool UserHasFlag(IUser user, string flag)
        {
            if (user.Equals(this.appConfiguration.Owner))
            {
                return true;
            }

            if(flag == Flag.Standard)
            {
                return true;
            }
            
            return false;
        }

        public IEnumerable<string> GetFlagsForUser(IUser user)
        {
            if (user.Equals(this.appConfiguration.Owner))
            {
                return new[]
                {
                    Flag.Access,
                    Flag.Configuration,
                    Flag.Debug,
                    Flag.Owner,
                    Flag.Protected,
                    Flag.Standard
                };
            }

            return new[] {Flag.Standard};
        }
    }
}