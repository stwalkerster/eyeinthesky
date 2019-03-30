namespace EyeInTheSky.Services.Web.Modules
{
    using Nancy;

    public class DefaultModule : NancyModule
    {
        public DefaultModule()
        {
            this.Get["/"] = _ => "Hello from Nancy!";
        }
    }
}