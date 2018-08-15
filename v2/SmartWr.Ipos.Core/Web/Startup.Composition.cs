using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using SmartWr.WebFramework.Library.Infrastructure.IoCs.Autofac;
using SmartWr.WebFramework.Library.Infrastructure.TypeFinder;

namespace SmartWr.Ipos.Core.Web
{
    public partial class Startup
    {
        public static void ConfigureComposition(HttpConfiguration config)
        {
            IContainer container = RegisterServices();
            var dependencyResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(dependencyResolver);
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = webApiResolver;
        }

        private static IContainer RegisterServices()
        {
            var container = EngineContext.Current.ContainerManager;

            LoadTypes(container);
            return container.Container;
        }

        private static void LoadTypes(IContainerManager containerMgr)
        {
            containerMgr.UpdateContainer(x =>
            {
                ITypeFinder typeFinder = new WebAppTypeFinder(true);
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = new List<IDependencyRegistrar>();

                foreach (var drType in drTypes)
                    drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });
        }
    }

}
