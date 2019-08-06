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
        private readonly IChannelConfiguration channelConfiguration;
        private IrcUserMask ownerMask;

        public UserFlagService(
            IAppConfiguration appConfiguration,
            IBotUserConfiguration userConfiguration,
            IChannelConfiguration channelConfiguration)
        {
            this.appConfiguration = appConfiguration;
            this.userConfiguration = userConfiguration;
            this.channelConfiguration = channelConfiguration;
        }

        public bool UserHasFlag(IUser iuser, string flag, string locality)
        {
            var user = iuser as IrcUser;
            
            if (user == null)
            {
                return false;
            }
            
            this.PreCacheOwnerMask(user);

            if (this.ownerMask.Matches(user).GetValueOrDefault(false))
            {
                return true;
            }

            if (flag == Flag.Standard)
            {
                return true;
            }

            if (user.Account != null && flag == AccessFlags.User)
            {
                return true;
            }
            
            var matchingUsers = this.userConfiguration.Items
                .Where(x => x.Mask.Matches(user).GetValueOrDefault())
                .ToList();

            foreach (var u in matchingUsers)
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

            // must be in a channel for local flags
            if (locality == null || !locality.StartsWith("#"))
            {
                return false;
            }

            var matchingChannelUsers = this.channelConfiguration[locality]
                .Users.Where(x => x.Mask.Matches(user).GetValueOrDefault())
                .ToList();

            foreach (var u in matchingChannelUsers)
            {
                if (u.LocalFlags == null)
                {
                    continue;
                }

                if (u.LocalFlags.Contains(flag))
                {
                    return true;
                }
            }
            
            // chanops
            if (user.Nickname != null 
                && user.Client.Channels[locality].Users.ContainsKey(user.Nickname) 
                && user.Client.Channels[locality].Users[user.Nickname].Operator)
            {
                if (flag == AccessFlags.LocalAdmin || flag == AccessFlags.Configuration)
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

        public IEnumerable<string> GetFlagsForUser(IUser iuser, string locality)
        {
            var user = iuser as IrcUser;
            
            if (user == null)
            {
                return new string[0];
            }
            
            this.PreCacheOwnerMask(user);

            var flags = new HashSet<string>{Flag.Standard};

            if (user.Account != null)
            {
                flags.Add(AccessFlags.User);
            }
            
            if (this.ownerMask.Matches(user).GetValueOrDefault(false))
            {
                flags.Add(Flag.Owner);
                flags.Add(AccessFlags.Configuration);
                flags.Add(AccessFlags.GlobalAdmin);
            }

            flags = this.userConfiguration.Items
                .Where(x => x.Mask.Matches(user).GetValueOrDefault() && x.GlobalFlags != null)
                .Select(x => x.GlobalFlags.ToCharArray())
                .Aggregate(
                    flags,
                    (cur, next) =>
                    {
                        foreach (var n in next)
                        {
                            cur.Add(n.ToString());
                        }

                        return cur;
                    });

            if (locality.StartsWith("#"))
            {
                flags = this.channelConfiguration[locality]
                    .Users.Where(x => x.Mask.Matches(user).GetValueOrDefault() && x.LocalFlags != null)
                    .Select(x => x.LocalFlags.ToCharArray())
                    .Aggregate(
                        flags,
                        (cur, next) =>
                        {
                            foreach (var n in next)
                            {
                                cur.Add(n.ToString());
                            }

                            return cur;
                        });
                
                // chanops
                if (user.Client.Channels[locality].Users[user.Nickname].Operator)
                {
                    flags.Add(AccessFlags.LocalAdmin);
                    flags.Add(AccessFlags.Configuration);
                }
            }

            return flags.ToList().OrderBy(x => x);
        }
    }
}