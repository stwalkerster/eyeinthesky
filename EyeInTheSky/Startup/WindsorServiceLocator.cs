namespace EyeInTheSky.Startup
{
    using System;
    using System.Collections.Generic;
    using Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    internal class WindsorServiceLocator : ServiceLocatorImplBase
    {
        private readonly IWindsorContainer container;

        public WindsorServiceLocator(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                return this.container.Resolve(key, serviceType);
            }

            return this.container.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (object[])this.container.ResolveAll(serviceType);
        }
    }
}