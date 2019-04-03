namespace EyeInTheSky.Web.Modules
{
    using System.Linq;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Models;
    using Stwalkerster.IrcClient.Interfaces;

    public class UsersModule : AuthenticatedModuleBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;

        public UsersModule(
            IAppConfiguration appConfiguration,
            IIrcClient freenodeClient,
            IBotUserConfiguration botUserConfiguration) : base(
            appConfiguration,
            freenodeClient)
        {
            // this.RequiresClaims(AccessFlags.GlobalAdmin);
            
            this.botUserConfiguration = botUserConfiguration;
            this.Get["/users"] = this.GetUserList;
        }

        private dynamic GetUserList(dynamic parameters)
        {
            var getUserListModel = this.CreateModel<GetUserListModel>(this.Context);
            getUserListModel.BotUsers = this.botUserConfiguration.Items.ToList();

            return getUserListModel;
        }
    }
}