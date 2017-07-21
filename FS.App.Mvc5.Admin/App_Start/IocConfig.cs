using Autofac;
using Autofac.Integration.Mvc;
using FS.Infrastructure.DependencyResolution;
using System.Reflection;
using System.Web.Mvc;

namespace FS.App.Mvc5.Admin
{
    public class IocConfig
    {
        public static void ResolveDependencies()
        {
            var builder = new ContainerBuilder();

            AutofacDependencyResolution.RegisterTypes(builder);

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}