namespace EyeInTheSky.Web.Modules
{
    using EyeInTheSky.Web.Misc;
    using EyeInTheSky.Web.Models;

    using Nancy;
    using Nancy.Security;

    public abstract class AuthenticatedModuleBase : NancyModule
    {
        protected AuthenticatedModuleBase()
        {
            this.RequiresAuthentication();
        }

        protected T CreateModel<T>(NancyContext context) where T : ModelBase, new()
        {
            var model = new T();
            var userIdentity = context.CurrentUser as UserIdentity;
            model.BotUser = userIdentity.BotUser;

            return model;
        }
    }
}