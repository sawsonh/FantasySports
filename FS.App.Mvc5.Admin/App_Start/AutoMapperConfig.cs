using System.Collections.Generic;
using AutoMapper;
using FS.App.Mvc5.Admin.Areas.Nba.Models;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Entities;
using FS.Infrastructure.DependencyResolution;

namespace FS.App.Mvc5.Admin
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            var profiles = new List<Profile>
            {
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

            Mapper.CreateMap<vNbaGame, NbaGameViewModel>()
                .ForMember(d => d.Quarter, m => m.MapFrom(s => s.PeriodValue))
                .ForMember(d => d.Status, m => m.MapFrom(s => s.PeriodStatus))
                .ForMember(d => d.Visitor, m => m.MapFrom(s => s.VisitorTeamNickname))
                .ForMember(d => d.Home, m => m.MapFrom(s => s.HomeTeamNickname));

            Mapper.CreateMap<NbaTeam, NbaTeamViewModel>();

            Mapper.CreateMap<AppSetting, ConfigurationViewModel>()
                .ForMember(d => d.Name, m => m.MapFrom(s => s.KeyName));

            Mapper.CreateMap<Game, GameViewModel>();

            Mapper.CreateMap<League, GameLeagueViewModel>()
                .ForMember(d => d.LeagueId, m => m.MapFrom(s => s.Id));

            Mapper.CreateMap<Period, GamePeriodViewModel>()
                .ForMember(d => d.PeriodId, m => m.MapFrom(s => s.Id));

            Mapper.CreateMap<vNbaTeamStat, NbaTeamStatViewModel>();

            Mapper.CreateMap<IdentityProvider, IdentityProviderViewModel>();

            Mapper.CreateMap<vUser, UserViewModel>();

            Mapper.CreateMap<Entry, EntryViewModel>();

            // Models to Entities

            Mapper.CreateMap<NbaGameViewModel, vNbaGame>()
                .ForMember(d => d.PeriodValue, m => m.MapFrom(s => s.Quarter))
                .ForMember(d => d.PeriodStatus, m => m.MapFrom(s => s.Status))
                .ForMember(d => d.VisitorTeamNickname, m => m.MapFrom(s => s.Visitor))
                .ForMember(d => d.HomeTeamNickname, m => m.MapFrom(s => s.Home));

            Mapper.CreateMap<NbaTeamViewModel, NbaTeam>();

            Mapper.CreateMap<ConfigurationViewModel, AppSetting>()
                .ForMember(d => d.KeyName, m => m.MapFrom(s => s.Name));

            Mapper.CreateMap<GameViewModel, Game>();

            Mapper.CreateMap<GameLeagueViewModel, League>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.LeagueId));

            Mapper.CreateMap<GamePeriodViewModel, Period>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.PeriodId));

            Mapper.CreateMap<IdentityProviderViewModel, IdentityProvider>();

            Mapper.CreateMap<UserViewModel, vUser>();

            Mapper.CreateMap<EntryViewModel, Entry>();
        }
    }
}