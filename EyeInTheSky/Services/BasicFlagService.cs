namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class BasicFlagService : IFlagService
    {
        private readonly IAppConfiguration appConfiguration;
        private IrcUserMask ownerMask;

        public BasicFlagService(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public bool UserHasFlag(IUser user, string flag)
        {
            this.PreCacheOwnerMask(user);

            if (this.ownerMask.Matches(user).GetValueOrDefault(false))
            {
                return true;
            }

            if (flag == Flag.Standard)
            {
                return true;
            }

            return false;
        }

        private void PreCacheOwnerMask(IUser user)
        {
            if (this.ownerMask != null)
            {
                return;
            }

            lock (this)
            {
                if (this.ownerMask != null)
                {
                    return;
                }

                var ircUser = user as IrcUser;
                if (ircUser == null)
                {
                    return;
                }

                var client = ircUser.Client;
                this.ownerMask = new IrcUserMask(this.appConfiguration.Owner, client);
            }
        }

        public IEnumerable<string> GetFlagsForUser(IUser user)
        {
            this.PreCacheOwnerMask(user);
            
            if (this.ownerMask.Matches(user).GetValueOrDefault(false))
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