namespace EyeInTheSky.Web.Modules
{
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using BCrypt.Net;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Models;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Stwalkerster.IrcClient.Interfaces;

    public class LoginModule : NancyModule
    {
        private readonly IBotUserConfiguration botUserConfiguration;
        private readonly IAppConfiguration appConfiguration;
        private readonly IIrcClient freenodeClient;

        public LoginModule(IBotUserConfiguration botUserConfiguration, IAppConfiguration appConfiguration, IIrcClient freenodeClient)
        {
            this.botUserConfiguration = botUserConfiguration;
            this.appConfiguration = appConfiguration;
            this.freenodeClient = freenodeClient;

            this.Get["/login"] = this.LogIn;
            this.Get["/logout"] = this.LogOut;
            this.Post["/login"] = this.LogInPost;
        }

        public dynamic LogIn(dynamic parameters)
        {
            return this.CreateModel();
        }
        
        public dynamic LogInPost(dynamic parameters)
        {
            var username = (string)this.Request.Form.username;
            var password = (string)this.Request.Form.password;

            var user = this.botUserConfiguration.Items.FirstOrDefault(x => x.Identifier == "$a:" + username);

            if (user != null)
            {
                if (BCrypt.Verify(password, user.WebPassword))
                {
                    var token = user.WebGuid;
                    return this.LoginAndRedirect(token);
                }
            }

            var loginDataModel = this.CreateModel();
            loginDataModel.Error = "Invalid username or password";
            loginDataModel.Username = username;
            return loginDataModel;
        }

        public dynamic LogOut(dynamic parameters)
        {
            return this.LogoutAndRedirect("~/");
        }

        public LoginDataModel CreateModel()
        {
            var ldm = new LoginDataModel();
            ldm.AppConfiguration = this.appConfiguration;
            ldm.IrcClient = this.freenodeClient;
            ldm.Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            return ldm;
        }
    }
}