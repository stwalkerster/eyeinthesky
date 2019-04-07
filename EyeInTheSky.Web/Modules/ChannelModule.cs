namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;

    using Nancy;
    using Stwalkerster.Bot.CommandLib.Model;
    using Stwalkerster.IrcClient.Interfaces;
    using Stwalkerster.IrcClient.Model;

    public class ChannelModule : AuthenticatedModuleBase
    {
        private readonly IChannelConfiguration channelConfiguration;
        private readonly IBotUserConfiguration botUserConfiguration;

        public ChannelModule(
            IAppConfiguration appConfiguration,
            IIrcClient freenodeClient,
            IChannelConfiguration channelConfiguration, IBotUserConfiguration botUserConfiguration
            )
            : base(appConfiguration, freenodeClient)
        {
            this.channelConfiguration = channelConfiguration;
            this.botUserConfiguration = botUserConfiguration;

            this.Get["/channels"] = this.GetChannelList;
            this.Get["/channels/{channel}"] = this.GetChannelInfo;
            this.Get["/channels/{channel}/stalk/{stalk}"] = this.GetStalkInfo;
        }

        public dynamic GetChannelList(dynamic parameters)
        {
            var getChannelListModel = this.CreateModel<GetChannelListModel>(this.Context);
            getChannelListModel.Channels = this.channelConfiguration.Items.ToList();

            return getChannelListModel;
        }

        public dynamic GetChannelInfo(dynamic parameters)
        {
            var channel = this.channelConfiguration.Items.FirstOrDefault(x => x.Guid == parameters.channel);

            if (channel == null)
            {
                return new NotFoundResponse();
            }

            var model = this.CreateModel<ChannelInfoModel>(this.Context);
            model.IrcChannel = channel;

            model.Stalks = channel.Stalks.Values.Select(v => new DisplayStalk(v, this.AppConfiguration)).ToList();

            if (model.IrcClient.Channels.ContainsKey(channel.Identifier))
            {
                var ircClientChannel = model.IrcClient.Channels[channel.Identifier];

                model.ChannelMembers = ircClientChannel.Users.Values.ToList();
            }
            else
            {
                model.ChannelMembers = new List<IrcChannelUser>();
                model.Errors.Add(string.Format("{0} is not currently in this channel, or tracking is broken.",
                    model.IrcClient.Nickname));
            }

            // //////////////////////////////////////////////////////////////////////////////////////////////////////

            var displayUsers = new List<ChannelDisplayUser>();

            var globalUsers = this.botUserConfiguration.Items.ToList();
            var localUsers = channel.Users.ToList();
            var channelMembers = model.ChannelMembers.ToList();

            foreach (var member in channelMembers)
            {
                var found = false;

                foreach (var user in channel.Users.ToList())
                {
                    if (user.Mask.Matches(member.User).GetValueOrDefault())
                    {
                        var u = new ChannelDisplayUser();
                        u.Member = member;
                        u.LocalUser = user;
                        u.GlobalUser = this.botUserConfiguration.Items.FirstOrDefault(x => Equals(x.Mask, user.Mask));
                        u.Construction = "ML";

                        localUsers.Remove(user);
                        globalUsers.RemoveAll(x => Equals(x.Mask, user.Mask));

                        found = true;
                        displayUsers.Add(u);
                        break;
                    }
                }

                if (!found)
                {
                    foreach (var user in this.botUserConfiguration.Items.ToList())
                    {
                        if (user.Mask.Matches(member.User).GetValueOrDefault())
                        {
                            var u = new ChannelDisplayUser();
                            u.Member = member;
                            u.GlobalUser = user;
                            u.Construction = "MG";
                            globalUsers.Remove(user);

                            found = true;
                            displayUsers.Add(u);
                            break;
                        }
                    }
                }

                if (!found)
                {
                    var u = new ChannelDisplayUser();
                    u.Member = member;
                    u.Construction = "M-";

                    displayUsers.Add(u);
                }
            }

            foreach (var user in localUsers)
            {
                var u = new ChannelDisplayUser();
                u.LocalUser = user;
                u.GlobalUser = this.botUserConfiguration.Items.FirstOrDefault(x => Equals(x.Mask, user.Mask));
                u.Construction = "-L";

                globalUsers.Remove(u.GlobalUser);

                displayUsers.Add(u);
            }

            // Some global users should be listed here since they have the ability to change bot config on a local basis
            foreach (var user in globalUsers.Where(x => !string.IsNullOrWhiteSpace(x.GlobalFlags))
                .Where(
                    x => x.GlobalFlags.Contains(Flag.Owner)
                         || x.GlobalFlags.Contains(AccessFlags.GlobalAdmin)
                         || x.GlobalFlags.Contains(AccessFlags.Configuration)))
            {
                var u = new ChannelDisplayUser();
                u.GlobalUser = this.botUserConfiguration.Items.FirstOrDefault(x => Equals(x.Mask, user.Mask));
                u.Construction = "-G";

                displayUsers.Add(u);
            }




            model.DisplayUsers = displayUsers;

            // //////////////////////////////////////////////////////////////////////////////////////////////////////

            return model;
        }

        public dynamic GetStalkInfo(dynamic parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}