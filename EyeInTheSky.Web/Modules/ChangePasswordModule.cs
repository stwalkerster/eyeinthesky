namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;
    using Stwalkerster.IrcClient.Interfaces;
    using BCrypt.Net;
    using Nancy;

    public class ChangePasswordModule : AuthenticatedModuleBase
    {
        private readonly IBotUserConfiguration botUserConfiguration;

        public ChangePasswordModule(IAppConfiguration appConfiguration, IIrcClient freenodeClient, IBotUserConfiguration botUserConfiguration) : base(
            appConfiguration,
            freenodeClient)
        {
            this.botUserConfiguration = botUserConfiguration;
            
            this.Get["/changepassword"] = this.ChangePassword;
            this.Post["/changepassword"] = this.ChangePasswordPost;
        }
        
        public dynamic ChangePassword(dynamic parameters)
        {
            return this.CreateModel<ChangePasswordDataModel>(this.Context);
        }
        
        public dynamic ChangePasswordPost(dynamic parameters)
        {
            var datamodel = this.CreateModel<ChangePasswordDataModel>(this.Context);

            var oldPassword = (string)this.Request.Form.oldPassword;
            var newPassword = (string)this.Request.Form.newPassword;
            var confirmPassword = (string)this.Request.Form.confirmPassword;

            var user = ((UserIdentity) this.Context.CurrentUser).BotUser;
            
            if (!BCrypt.Verify(oldPassword, user.WebPassword))
            {
                datamodel.Errors = new List<string> {"Invalid password"};
                return datamodel;
            }
            
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                datamodel.Errors = new List<string>
                {
                    string.Format(
                        "Your new password cannot be empty. If you wish to remove your access to the web UI, you will need to run \"{0}account webpass\" from IRC",
                        this.AppConfiguration.CommandPrefix)
                };
                return datamodel;
            }
            
            if (newPassword != confirmPassword)
            {
                datamodel.Errors = new List<string> {"Passwords do not match."};
                return datamodel;
            }
            
            user.WebPassword = BCrypt.HashPassword(newPassword);
            this.botUserConfiguration.Save();

            return this.Response.AsRedirect("/");
        }
    }
}