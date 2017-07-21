using Autofac;
using FS.Core.Repositories;
using FS.Core.Services;
using FS.Infrastructure.Repositories;
using FS.Infrastructure.Services;

namespace FS.Infrastructure.DependencyResolution
{
    public class AutofacDependencyResolution
    {
        public static void RegisterTypes(ContainerBuilder builder)
        {
            RegisterGeneric(typeof(IDataService<>), typeof(DataService<>), builder, false);

            RegisterType<IHttpService, HttpService>(builder);

            RegisterType<IConfigService, ConfigService>(builder);

            RegisterType<ILogService, LogService>(builder);

            RegisterType<IGameService, GameService>(builder);

            RegisterType<INbaService, NbaService>(builder);

            RegisterType<IIdentityProviderService, IdentityProviderService>(builder);

            RegisterType<IUserService, UserService>(builder);

            RegisterType<IEntryService, EntryService>(builder);

            RegisterGeneric(typeof(IDataRepository<>), typeof(DataRepository<>), builder, false);

            RegisterType<ISqlRepository, SqlRepository>(builder);

            RegisterType<IGameRepository, GameRepository>(builder);

            RegisterType<IPeriodRepository, PeriodRepository>(builder);
        }
        
        public static void RegisterGeneric(System.Type service, System.Type implementer, ContainerBuilder builder, bool isSingleInstance)
        {
            builder.RegisterGeneric(implementer)
                .As(service);
        }

        public static void RegisterType<TService, TImplementer>(ContainerBuilder builder)
        {
            builder.RegisterType<TImplementer>()
                .As<TService>();
        }
    }
}
