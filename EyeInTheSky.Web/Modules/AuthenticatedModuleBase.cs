namespace EyeInTheSky.Web.Modules
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;

    using Nancy;
    using Nancy.Security;
    using Stwalkerster.IrcClient.Interfaces;

    public abstract class AuthenticatedModuleBase : NancyModule
    {
        public IAppConfiguration AppConfiguration { get; private set; }
        public IIrcClient FreenodeClient { get; private set; }

        protected AuthenticatedModuleBase(IAppConfiguration appConfiguration, IIrcClient freenodeClient)
        {
            this.AppConfiguration = appConfiguration;
            this.FreenodeClient = freenodeClient;
            this.RequiresAuthentication();
        }

        protected T CreateModel<T>(NancyContext context) where T : ModelBase, new()
        {
            var model = new T();
            var userIdentity = context.CurrentUser as UserIdentity;
            model.BotUser = userIdentity.BotUser;

            model.AppConfiguration = this.AppConfiguration;
            model.IrcClient = this.FreenodeClient;
            model.Errors = new List<string>();

            model.Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            return model;
        }
    }
}
