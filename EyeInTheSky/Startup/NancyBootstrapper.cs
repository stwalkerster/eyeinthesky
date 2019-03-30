namespace EyeInTheSky.Startup
{
    using Castle.Windsor;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Windsor;

    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            FormsAuthenticationConfiguration formsAuthConfiguration = new FormsAuthenticationConfiguration();
            
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

        }
    }
}