using FS.Core.Entities;
using FS.Core.Repositories;
using System.Data.SqlClient;

namespace FS.Infrastructure.Repositories
{
    public class GameRepository : DataRepository<Game>, IGameRepository
    {
        private readonly ISqlRepository _sqlRepo;

        public GameRepository(ISqlRepository sqlRepo)
        {
            _sqlRepo = sqlRepo;
        }

        public void Delete(int id)
        {
            _sqlRepo.ExecuteSQL("DELETE FROM Games WHERE Id = @Id", new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@Id", Value = id }
            });
        }

        public void RemoveLeague(int id, int leagueId)
        {
            _sqlRepo.ExecuteSQL("DELETE FROM GameLeagues WHERE GameId = @GameId AND LeagueId = @LeagueId", new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@GameId", Value = id },
                new SqlParameter { ParameterName = "@LeagueId", Value = leagueId }
            });
        }

        public void AddLeague(int id, int leagueId)
        {
            _sqlRepo.ExecuteSQL("INSERT INTO GameLeagues (GameId, LeagueId) VALUES (@GameId, @LeagueId)", new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@GameId", Value = id },
                new SqlParameter { ParameterName = "@LeagueId", Value = leagueId }
            });
        }
    }
}
