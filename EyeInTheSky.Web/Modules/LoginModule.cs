namespace EyeInTheSky.Web.Modules
{
    using System;
    using EyeInTheSky.Web.Models;
    using Nancy;
    using Nancy.Authentication.Forms;

    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
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

            if (true /* user is valid */)
            {
                var token = Guid.NewGuid() ;
                return this.LoginAndRedirect(token);
            }
            else
            {
                throw new ArgumentException("Invalid username or password");
            }
        }

        public dynamic LogOut(dynamic parameters)
        {
            return this.LogoutAndRedirect("~/");
        }
    }
}