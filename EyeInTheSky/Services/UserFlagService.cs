namespace EyeInTheSky.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.Bot.CommandLib.Services.Interfaces;
    using Stwalkerster.IrcClient.Model;
    using Stwalkerster.IrcClient.Model.Interfaces;

    public class UserFlagService : IFlagService
    {
        private readonly IAppConfiguration appConfiguration;
        private readonly IBotUserConfiguration userConfiguration;
        private IrcUserMask ownerMask;

        public UserFlagService(IAppConfiguration appConfiguration, IBotUserConfiguration userConfiguration)
        {
            this.appConfiguration = appConfiguration;
            this.userConfiguration = userConfiguration;
        }

        public bool UserHasFlag(IUser user, string flag, string locality)
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

            var matchingUsers = this.userConfiguration.Items
                .Where(x => x.Mask.Matches(user).GetValueOrDefault())
                .ToList();
            
            foreach(var u in matchingUsers)
            {
                if (u.GlobalFlags == null)
                {
                    continue;
                }
                
                if (u.GlobalFlags.Contains(flag))
                {
                    return true;
                }
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

        public IEnumerable<string> GetFlagsForUser(IUser user, string locality)
        {
            this.PreCacheOwnerMask(user);
            
            if (this.ownerMask.Matches(user).GetValueOrDefault(false))
            {
                return new[]
                {
                    Flag.Owner,
                    AccessFlags.Configuration,
                    Flag.Standard,
                    AccessFlags.Admin,
                }.OrderBy(x => x);
            }

            var flags = this.userConfiguration.Items
                .Where(x => x.Mask.Matches(user).GetValueOrDefault())
                .Select(x => x.GlobalFlags.ToCharArray())
                .Aggregate(
                    new HashSet<string> {Flag.Standard},
                    (cur, next) =>
                    {
                        foreach (var n in next)
                        {
                            cur.Add(n.ToString());
                        }

                        return cur;
                    })
                .ToList();

            return flags.OrderBy(x => x);
        }
    }
}