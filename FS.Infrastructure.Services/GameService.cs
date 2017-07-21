using FS.Core.Services;
using System;
using FS.Core.Entities;
using System.Collections.Generic;
using FS.Core.Repositories;
using System.Linq;

namespace FS.Infrastructure.Services
{
    public class GameService : DataService<Game>, IGameService
    {
        private readonly IGameRepository _repo;
        private readonly IDataRepository<League> _lgRepo;
        private readonly IPeriodRepository _prdRepo;

        public GameService(IGameRepository repo, IDataRepository<League> lgRepo, IPeriodRepository prdRepo)
            : base(repo)
        {
            _repo = repo;
            _lgRepo = lgRepo;
            _prdRepo = prdRepo;
        }

        public override void Update(params Game[] games)
        {
            foreach (var game in games)
            {
                if (DateTime.Compare(game.RegistrationStartDate, game.RegistrationEndDate) >= 0)
                    throw new Exception("Registration start date must be after its end date.");

                if (DateTime.Compare(game.PlayStartDate, game.PlayEndDate) >= 0)
                    throw new Exception("Play start date must be after its end date.");

                var entity = _repo.GetSingle(d => d.Id == game.Id);
                if (entity == null)
                {
                    throw new Exception($"Cannot find game for Id {game.Id}");
                }

                var hasChanged = !game.CompareTo(entity, "Id");
                if (hasChanged)
                {
                    game.CopyTo(entity, "Id");
                    _repo.Update(entity);
                }
            }
        }

        public override void Add(params Game[] games)
        {
            foreach (var game in games)
            {
                if (DateTime.Compare(game.RegistrationStartDate, game.RegistrationEndDate) >= 0)
                    throw new Exception("Registration start date must be after its end date.");

                if (DateTime.Compare(game.PlayStartDate, game.PlayEndDate) >= 0)
                    throw new Exception("Play start date must be after its end date.");

                var gameName = game.Name.Trim();
                var entity = _repo.GetSingle(d => d.Name.Trim().ToLower().Equals(gameName.ToLower()));
                if (entity != null)
                {
                    throw new Exception($"Cannot create game. '{gameName}' name was previously used.");
                }

                _repo.Add(game);
            }
        }

        public void Delete(int id)
        {
            var entity = _repo.GetSingle(d => d.Id == id);
            if (entity == null)
            {
                throw new Exception($"Cannot find game for Id {id}");
            }
            _repo.Delete(id);
        }

        public IEnumerable<League> GetLeagues(int id)
        {
            return _lgRepo.GetList(lg => lg.Games.Any(gm => gm.Id == id));
        }

        public void RemoveLeague(int id, int leagueId)
        {
            var entity = _lgRepo.GetSingle(lg => lg.Id == leagueId && lg.Games.Any(gm => gm.Id == id));
            if (entity == null)
            {
                throw new Exception($"Cannot find game for Id {id} or league Id {leagueId}");
            }
            _repo.RemoveLeague(id, leagueId);
        }

        public IEnumerable<League> GetLeaguesToAdd(int id)
        {
            return _lgRepo.GetList(lg => !lg.Games.Any(gm => gm.Id == id));
        }

        public void AddLeague(int id, int leagueId)
        {
            var game = _repo.GetSingle(gm => gm.Id == id);
            if (game == null)
            {
                throw new Exception($"Cannot find game for Id {id}");
            }

            var entity = _lgRepo.GetSingle(lg => lg.Games.Any(gm => gm.Id == id) && lg.Id == leagueId);
            if (entity != null)
            {
                throw new Exception($"Cannot add league to game. League Id '{leagueId}' was previously added.");
            }

            entity = _lgRepo.GetSingle(lg => lg.Id == leagueId);
            if (entity == null)
            {
                throw new Exception($"Cannot find league for Id {leagueId}");
            }
            _repo.AddLeague(id, leagueId);
        }

        public IEnumerable<Period> GetPeriods(int id)
        {
            return _prdRepo.GetList(period => period.GameId == id);
        }

        public void UpdatePeriod(int id, Period period)
        {
            if (DateTime.Compare(period.PickStartDateTime, period.PickEndDateTime) >= 0)
                throw new Exception($"Registration start date must be after its end date.");

            if (DateTime.Compare(period.ReportStartDateTime, period.ReportEndDateTime) >= 0)
                throw new Exception($"Play start date must be after its end date.");

            var entity = _prdRepo.GetSingle(prd => prd.GameId == id && prd.Id == period.Id);
            if (entity == null)
            {
                throw new Exception($"Cannot find game for Id {id} or period Id {period.Id}");
            }

            var hasChanged = !period.CompareTo(entity, "GameId");
            if (hasChanged)
            {
                period.CopyTo(entity, "GameId");
                _prdRepo.Update(entity);
            }
        }

        public void DeletePeriod(int id, int periodId)
        {
            var entity = _prdRepo.GetSingle(prd => prd.GameId == id && prd.Id == periodId);
            if (entity == null)
            {
                throw new Exception($"Cannot find game for Id {id} or period Id {periodId}");
            }
            _prdRepo.Remove(entity);
        }

        public void AddPeriod(int id, IEnumerable<Period> periods)
        {
            var mutablePeriods = periods.ToList();
            mutablePeriods.ForEach(period =>
            {
                if (string.IsNullOrEmpty(period.Name))
                    throw new Exception("Period name is required");

                if (period.PickStartDateTime == default(DateTime))
                    throw new Exception("Pick start date is required");

                if (period.PickEndDateTime == default(DateTime))
                    throw new Exception("Pick end date is required");

                if (period.ReportStartDateTime == default(DateTime))
                    throw new Exception("Report start date is required");

                if (period.ReportEndDateTime == default(DateTime))
                    throw new Exception("Report end date is required");

                if (DateTime.Compare(period.PickStartDateTime, period.PickEndDateTime) >= 0)
                    throw new Exception("Pick start date must be after its end date.");

                if (DateTime.Compare(period.ReportStartDateTime, period.ReportEndDateTime) >= 0)
                    throw new Exception("Report start date must be after its end date.");

                period.GameId = id;
            });

            _prdRepo.Add(mutablePeriods.ToArray());
        }
    }
}
