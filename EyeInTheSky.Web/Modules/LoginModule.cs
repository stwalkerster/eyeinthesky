namespace EyeInTheSky.Web.Modules
{
    using System.Linq;

    using BCrypt.Net;

    using EyeInTheSky.Services.Interfaces;
    using EyeInTheSky.Web.Models;
    using Nancy;
    using Nancy.Authentication.Forms;

    public class LoginModule : NancyModule
    {
        private readonly IBotUserConfiguration botUserConfiguration;

        public LoginModule(IBotUserConfiguration botUserConfiguration)
        {
            this.botUserConfiguration = botUserConfiguration;
            
            this.Get["/login"] = this.LogIn;
            this.Get["/logout"] = this.LogOut;
            this.Post["/login"] = this.LogInPost;
        }

        public dynamic LogIn(dynamic parameters)
        {
            return new LoginDataModel();
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

            return new LoginDataModel {Error = "Invalid username or password", Username = username};
        }

        public dynamic LogOut(dynamic parameters)
        {
            return this.LogoutAndRedirect("~/");
        }
    }
}