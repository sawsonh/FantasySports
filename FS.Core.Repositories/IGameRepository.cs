using FS.Core.Entities;

namespace FS.Core.Repositories
{
    public interface IGameRepository : IDataRepository<Game>
    {
        void Delete(int id);
        void RemoveLeague(int id, int leagueId);
        void AddLeague(int id, int leagueId);
    }
}
