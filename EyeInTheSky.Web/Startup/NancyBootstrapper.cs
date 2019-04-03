namespace EyeInTheSky.Web.Startup
{
    using System;
    using System.Reflection;
    using Castle.Core.Logging;
    using Castle.Windsor;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.Bootstrappers.Windsor;
    using Nancy.Security;
    using Nancy.ViewEngines;

    public class NancyBootstrapper : WindsorNancyBootstrapper
    {
        private ILogger logger;

        protected override void RequestStartup(IWindsorContainer container, IPipelines pipelines, NancyContext context)
        {
            SSLProxy.RewriteSchemeUsingForwardedHeaders(pipelines);

            FormsAuthenticationConfiguration formsAuthConfiguration;           
            try
            {
                var userMapper = container.Resolve<IUserMapper>();
            
                formsAuthConfiguration = new FormsAuthenticationConfiguration
                {
                    UserMapper = userMapper,
                    RedirectUrl = "~/login"
                };
            }
            catch (Exception e)
            {
                this.logger.Error("Error getting user mapper", e);
                throw;
            }

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }

        protected override void ApplicationStartup(IWindsorContainer container, IPipelines pipelines)
        {
            ResourceViewLocationProvider.RootNamespaces.Add(Assembly.GetExecutingAssembly(), "EyeInTheSky.Web");

            this.logger = container.Resolve<ILogger>();
            
            base.ApplicationStartup(container, pipelines);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(
                    nic => nic.ViewLocationProvider = typeof(ResourceViewLocationProvider));
            }
        }

        protected override IWindsorContainer GetApplicationContainer()
        {
            var typeName = Assembly.GetEntryAssembly().GetName().Name + ".Startup.Launch";
            var type = Assembly.GetEntryAssembly().GetType(typeName);

            var fieldInfo = type.GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo == null)
            {
                throw new Exception("Cannot find container");
            }
            
            var windsorContainer = (IWindsorContainer)fieldInfo.GetValue(null);
            return windsorContainer;
        }
    }
}