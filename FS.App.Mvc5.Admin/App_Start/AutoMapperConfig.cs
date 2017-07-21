using AutoMapper;
using FS.App.Mvc5.Admin.Areas.Nba.Models;
using FS.App.Mvc5.Admin.Models;
using FS.Infrastructure.DependencyResolution;
using System.Collections.Generic;

namespace FS.App.Mvc5.Admin
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            var profiles = new List<Profile>{
                new AutoMapperApiProfile()
            };
            AutomapperDependencyResolution.Configure(profiles);
        }
    }

    public class AutoMapperApiProfile : Profile
    {
        protected override void Configure()
        {
            // Entities to Models

            Mapper.CreateMap<Core.Entities.vNbaGame, NbaGameViewModel>()
                .ForMember(d => d.Quarter, m => m.MapFrom(s => s.PeriodValue))
                .ForMember(d => d.Status, m => m.MapFrom(s => s.PeriodStatus))
                .ForMember(d => d.Visitor, m => m.MapFrom(s => s.VisitorTeamNickname))
                .ForMember(d => d.Home, m => m.MapFrom(s => s.HomeTeamNickname));

            Mapper.CreateMap<Core.Entities.NbaTeam, NbaTeamViewModel>();

            Mapper.CreateMap<Core.Entities.AppSetting, ConfigurationViewModel>()
                .ForMember(d => d.Name, m => m.MapFrom(s => s.KeyName));

            Mapper.CreateMap<Core.Entities.Game, GameViewModel>();

            Mapper.CreateMap<Core.Entities.League, GameLeagueViewModel>()
                .ForMember(d => d.LeagueId, m => m.MapFrom(s => s.Id));

            Mapper.CreateMap<Core.Entities.Period, GamePeriodViewModel>()
                .ForMember(d => d.PeriodId, m => m.MapFrom(s => s.Id));

            Mapper.CreateMap<Core.Entities.vNbaTeamStat, NbaTeamStatViewModel>();

            Mapper.CreateMap<Core.Entities.IdentityProvider, IdentityProviderViewModel>();

            Mapper.CreateMap<Core.Entities.vUser, UserViewModel>();

            Mapper.CreateMap<Core.Entities.Entry, EntryViewModel>();

            // Models to Entities

            Mapper.CreateMap<NbaGameViewModel, Core.Entities.vNbaGame>()
                .ForMember(d => d.PeriodValue, m => m.MapFrom(s => s.Quarter))
                .ForMember(d => d.PeriodStatus, m => m.MapFrom(s => s.Status))
                .ForMember(d => d.VisitorTeamNickname, m => m.MapFrom(s => s.Visitor))
                .ForMember(d => d.HomeTeamNickname, m => m.MapFrom(s => s.Home));

            Mapper.CreateMap<NbaTeamViewModel, Core.Entities.NbaTeam>();

            Mapper.CreateMap<ConfigurationViewModel, Core.Entities.AppSetting>()
                .ForMember(d => d.KeyName, m => m.MapFrom(s => s.Name));

            Mapper.CreateMap<GameViewModel, Core.Entities.Game>();

            Mapper.CreateMap<GameLeagueViewModel, Core.Entities.League>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.LeagueId));

            Mapper.CreateMap<GamePeriodViewModel, Core.Entities.Period>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PeriodId));

            Mapper.CreateMap<IdentityProviderViewModel, Core.Entities.IdentityProvider>();

            Mapper.CreateMap<UserViewModel, Core.Entities.vUser>();

            Mapper.CreateMap<EntryViewModel, Core.Entities.Entry>();

        }
    }
}