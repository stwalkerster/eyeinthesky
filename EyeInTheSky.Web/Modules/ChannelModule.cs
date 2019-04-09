namespace EyeInTheSky.Web.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
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
        private readonly IStalkNodeFactory stalkNodeFactory;

        public ChannelModule(
            IAppConfiguration appConfiguration,
            IIrcClient freenodeClient,
            IChannelConfiguration channelConfiguration,
            IBotUserConfiguration botUserConfiguration,
            IStalkNodeFactory stalkNodeFactory
        )
            : base(appConfiguration, freenodeClient)
        {
            this.channelConfiguration = channelConfiguration;
            this.botUserConfiguration = botUserConfiguration;
            this.stalkNodeFactory = stalkNodeFactory;

            this.Get["/channels"] = this.GetChannelList;
            this.Get["/channels/{channel}"] = this.GetChannelInfo;
            this.Get["/channels/{channel}/stalk/{stalk}"] = this.GetStalkInfo;
            this.Get["/channels/{channel}/stalk/{stalk}/edit"] = this.GetStalkInfoForEdit;
            this.Post["/channels/{channel}/stalk/{stalk}/edit"] = this.PostStalkInfo;
        }

        #region Route handlers

        public dynamic PostStalkInfo(dynamic parameters)
        {
            var channel = this.channelConfiguration.Items.FirstOrDefault(x => x.Guid == parameters.channel);

            if (channel == null || !channel.Stalks.ContainsKey(parameters.stalk))
            {
                return new NotFoundResponse();
            }

            var doc = new XmlDocument();
            doc.LoadXml(this.Request.Form.newsearchtree);

            IStalk stalk = channel.Stalks[parameters.stalk];
            var newTree = this.stalkNodeFactory.NewFromXmlFragment((XmlElement) doc.FirstChild.FirstChild);

            stalk.SearchTree = newTree;

            var stalkIsEnabled = this.Request.Form.enabledSwitch;

            stalk.IsEnabled = stalkIsEnabled.HasValue;
            stalk.Description = this.Request.Form.stalkDescription;

            var expiry = this.Request.Form.expiry;

            DateTime newExpiry;
            var dateParseResult = DateTime.TryParse(expiry, out newExpiry);
            if (dateParseResult)
            {
                stalk.ExpiryTime = newExpiry.ToUniversalTime();
            }
            else
            {
                stalk.ExpiryTime = null;
            }

            this.channelConfiguration.Save();
            this.FreenodeClient.SendMessage(
                stalk.Channel,
                "The stalk " + stalk.Identifier + " was modified by " + this.Context.CurrentUser.UserName
                + " from the web interface.");

            return this.Response.AsRedirect(string.Format("/channels/{0}/stalk/{1}", channel.Guid, stalk.Identifier));
        }

        public dynamic GetChannelList(dynamic parameters)
        {
            var getChannelListModel = this.CreateModel<GetChannelListModel>(this.Context);
            getChannelListModel.Channels = this.channelConfiguration.Items
                .Where(x => this.UserCanSeeChannel(this.Context, x))
                .ToList();

            return getChannelListModel;
        }

        public dynamic GetChannelInfo(dynamic parameters)
        {
            var channel = this.channelConfiguration.Items.FirstOrDefault(x => x.Guid == parameters.channel);

            if (channel == null)
            {
                return new NotFoundResponse();
            }

            if (!this.UserCanSeeChannel(this.Context, channel))
            {
                return new NotFoundResponse();
            }

            var model = this.CreateModel<ChannelInfoModel>(this.Context);
            model.IrcChannel = channel;

            model.Stalks = channel.Stalks.Values
                .Select(v => new DisplayStalk(v, this.AppConfiguration, this.stalkNodeFactory))
                .ToList();

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

            model.DisplayUsers = this.GetChannelDisplayUsers(channel, model);

            return model;
        }

        public dynamic GetStalkInfo(dynamic parameters)
        {
            var model = this.CreateModel<StalkInfoModel>(this.Context);
            return StalkInfoPageBase(parameters, model);
        }

        public dynamic GetStalkInfoForEdit(dynamic parameters)
        {
            var model = this.CreateModel<EditableStalkInfoModel>(this.Context);
            return StalkInfoPageBase(parameters, model);
        }

        #endregion

        private dynamic StalkInfoPageBase(dynamic parameters, StalkInfoModel model)
        {
            var channel = this.channelConfiguration.Items.FirstOrDefault(x => x.Guid == parameters.channel);

            if (channel == null || !channel.Stalks.ContainsKey(parameters.stalk))
            {
                return new NotFoundResponse();
            }

            if (!this.UserCanSeeChannel(this.Context, channel))
            {
                return new NotFoundResponse();
            }

            model.IrcChannel = channel;
            model.Stalk = new DisplayStalk(
                channel.Stalks[parameters.stalk],
                this.AppConfiguration,
                this.stalkNodeFactory);

            return model;
        }

        private List<ChannelDisplayUser> GetChannelDisplayUsers(IIrcChannel channel, ChannelInfoModel model)
        {
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
                         || x.GlobalFlags.Contains(AccessFlags.LocalAdmin)
                         || x.GlobalFlags.Contains(AccessFlags.Configuration)))
            {
                var u = new ChannelDisplayUser();
                u.GlobalUser = this.botUserConfiguration.Items.FirstOrDefault(x => Equals(x.Mask, user.Mask));
                u.Construction = "-G";

                displayUsers.Add(u);
            }

            return displayUsers;
        }

        private bool UserCanSeeChannel(NancyContext context, IIrcChannel channel)
        {
            /*
             * user is "aware" if:
             *   a) they are a member of the channel
             *   b) they are subscribed to the channel or a stalk within the channel
             *   c) they have config or localadmin flags in the channel
             *   d) they have globaladmin, localadmin, config, or owner flags globally
             */


            var currentUser = ((UserIdentity) context.CurrentUser).BotUser;
            // currentUser is "aware" if:

            // a) they are a member of the channel
            if (this.FreenodeClient.Channels.ContainsKey(channel.Identifier))
            {
                var ircClientChannel = this.FreenodeClient.Channels[channel.Identifier];

                if (ircClientChannel.Users.Values.Any(x => currentUser.Mask.Matches(x.User).GetValueOrDefault()))
                {
                    return true;
                }
            }

            // b) they are subscribed to the channel or a stalk within the channel
            if (channel.Users.Any(x => x.Subscribed && x.Mask.Equals(currentUser.Mask))
                || channel.Stalks.Values.Any(
                    s => s.Subscribers.Any(u => u.Subscribed && u.Mask.Equals(currentUser.Mask))))
            {
                return true;
            }

            // c) they have config or localadmin flags in the channel
            if (channel.Users.Any(
                x =>
                    !string.IsNullOrEmpty(x.LocalFlags)
                    && (x.LocalFlags.Contains(AccessFlags.LocalAdmin)
                        || x.LocalFlags.Contains(AccessFlags.Configuration))
                    && x.Mask.Equals(currentUser.Mask)))
            {
                return true;
            }

            // d) they have globaladmin, localadmin, config, or owner flags globally
            if (this.botUserConfiguration.Items.Any(
                x =>
                    x.Mask.Equals(currentUser.Mask)
                    && !string.IsNullOrEmpty(x.GlobalFlags)
                    && (x.GlobalFlags.Contains(AccessFlags.LocalAdmin)
                        || x.GlobalFlags.Contains(AccessFlags.GlobalAdmin)
                        || x.GlobalFlags.Contains(AccessFlags.Configuration)
                        || x.GlobalFlags.Contains(Flag.Owner))
            ))
            {
                return true;
            }

            return false;
        }

        private bool UserCanSeeChannelConfig(NancyContext context, IIrcChannel channel)
        {
            /*
             * user can see config if:
             *   a) they are a member of the channel
             *   b) they are subscribed to the channel or a stalk within the channel
             *   c) they have *local* config or localadmin flags in the channel
             *   d) they have owner flags
             *
             * Note: globaladmin does not permit viewing of the channel configuration.
             * Note: global config does not permit viewing of the channel configuration.
             *
             * This allows private channels to be protected, but still managed by global users if necessary.
             */

            if (channel.Identifier == "##stwalkerster-privalerts")
            {
                return false;
            }

            return true;
        }

        private bool UserCanConfigureStalks(NancyContext context, IIrcChannel channel)
        {
            /*
             * user can configure if:
             *   a) they have *local* config
             *   b) they have *global* config
             */
            return true;
        }
    }
}