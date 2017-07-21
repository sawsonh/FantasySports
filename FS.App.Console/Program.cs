using Autofac;
using FS.Core.Services;
using FS.Infrastructure.DependencyResolution;
using System;
using System.Linq;

namespace FS.App.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTime = DateTime.Now.AddHours(-6);
            if (args.Count() > 0)
                if (!DateTime.TryParse(args[0], out dateTime))
                    dateTime = DateTime.Now.AddHours(-6);

            var builder = new ContainerBuilder();

            AutofacDependencyResolution.RegisterTypes(builder);
            
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var nba = scope.Resolve<INbaService>();
                nba.Run(dateTime);
            }
        }

    }
}
