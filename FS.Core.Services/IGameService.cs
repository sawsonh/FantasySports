using FS.Core.Entities;
using System;
using System.Collections.Generic;

namespace FS.Core.Services
{
    public interface IGameService : IDataService<Game>
    {
        void Delete(int id);
        IEnumerable<League> GetLeagues(int id);
        void RemoveLeague(int id, int leagueId);
        IEnumerable<League> GetLeaguesToAdd(int id);
        void AddLeague(int id, int leagueId);
        IEnumerable<Period> GetPeriods(int id);
        void UpdatePeriod(int id, Period period);
        void DeletePeriod(int id, int periodId);
        void AddPeriod(int id, IEnumerable<Period> periods);
    }
}
