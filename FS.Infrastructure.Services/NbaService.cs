using FS.Core.Entities;
using FS.Core.Repositories;
using FS.Core.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace FS.Infrastructure.Services
{
    public class NbaService : INbaService
    {
        private readonly IHttpService _httpSvc;
        private readonly IConfigService _cfgSvc;
        private readonly ILogService _logSvc;
        private readonly ISqlRepository _sqlRepo;
        private readonly IDataRepository<vNbaGame> _repo;

        public NbaService(IHttpService httpSvc, ISqlRepository sqlSvc, IConfigService cfgSvc, ILogService logSvc, IDataRepository<vNbaGame> repo)
        {
            _httpSvc = httpSvc;
            _sqlRepo = sqlSvc;
            cfgSvc.AppId = 0;
            _cfgSvc = cfgSvc;
            _logSvc = logSvc;
            _repo = repo;
        }

        public void Run(DateTime dateTime)
        {
            var threadUpdateGames = new Thread(() => UpdateGames(dateTime));
            var threadUpdateTeamStats = new Thread(() => UpdateTeamStats());

            threadUpdateGames.Start();
            threadUpdateTeamStats.Start();
        }
        
        private void UpdateTeamStats()
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_TeamStats")) return;

            var nbaSeason = DateTime.Now.AddMonths(-8).Year;
            var nbaSeasonDesc = nbaSeason + "-" + ((nbaSeason + 1) % 1000);
            var data = _httpSvc.Get<string>("http://stats.nba.com/stats/leaguedashteamstats?Conference=&DateFrom=&DateTo=&Division=&GameScope=&GameSegment=&LastNGames=0&LeagueID=00&Location=&MeasureType=Base&Month=0&OpponentTeamID=0&Outcome=&PORound=0&PaceAdjust=N&PerMode=PerGame&Period=0&PlayerExperience=&PlayerPosition=&PlusMinus=N&Rank=N&Season={0}&SeasonSegment=&SeasonType=Regular+Season&ShotClockRange=&StarterBench=&TeamID=0&VsConference=&VsDivision=", nbaSeasonDesc);
            if (string.IsNullOrEmpty(data))
            {
                _logSvc.Info("UpdateTeamStats did not receive data for {0}", nbaSeasonDesc);
                return;
            }
            var json = JObject.Parse(data);

            SqlParameter[] sqlParameters;
            const string sqlTeamStats = @"
MERGE NbaTeamStats s
USING (SELECT @TeamId TeamId, @SeasonDesc SeasonDesc) src
ON s.TeamId = src.TeamId AND s.SeasonDesc = src.SeasonDesc
WHEN MATCHED THEN
UPDATE
SET GP = @GP, W = @W, L = @L, FGPCT = @FGPCT, FG3PCT = @FG3PCT, FTPCT = @FTPCT, OREB = @OREB, REB = @REB, AST = @AST, STL = @STL, BLK = @BLK, TOVR = @TOVR, PTS = @PTS
WHEN NOT MATCHED THEN
INSERT (TeamId, SeasonDesc, GP, W, L, FGPCT, FG3PCT, FTPCT, OREB, REB, AST, STL, BLK, TOVR, PTS)
VALUES (@TeamId, @SeasonDesc, @GP, @W, @L, @FGPCT, @FG3PCT, @FTPCT, @OREB, @REB, @AST, @STL, @BLK, @TOVR, @PTS);";

            var mapCustomColumns = new Dictionary<string, string>
            {
                { "TEAM_ID", "TeamId" }, { "FG_PCT", "FGPCT" }, { "FG3_PCT", "FG3PCT" }, { "FT_PCT", "FTPCT" }, { "TOV", "TOVR" }
            };

            foreach (var resultSet in json["resultSets"])
            {
                if (!((string)resultSet["name"]).Equals("LeagueDashTeamStats")) continue;
                var headers = resultSet["headers"].Select(header => mapCustomColumns.ContainsKey(((string)header).Trim()) ? mapCustomColumns[((string)header).Trim()] : ((string)header).Trim()).ToArray();
                sqlParameters = new SqlParameter[headers.Count() + 1];
                foreach (var teamStats in resultSet["rowSet"])
                {
                    for (var i = 0; i < headers.Count(); i++)
                    {
                        var value = GetJProperty<string>(teamStats, i);
                        sqlParameters[i] = string.IsNullOrEmpty(value)
                            ? new SqlParameter { ParameterName = "@" + headers[i], Value = DBNull.Value }
                            : new SqlParameter { ParameterName = "@" + headers[i], Value = value };
                    }
                    sqlParameters[headers.Count()] = new SqlParameter { ParameterName = "@SeasonDesc", Value = nbaSeasonDesc };
                    _sqlRepo.ExecuteSQL(sqlTeamStats, sqlParameters);
                }
            }
        }

        private void UpdateGames(DateTime dateTime)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_Games")) return;

            var gameDate = dateTime.ToString("yyyyMMdd");
            var data = _httpSvc.Get<string>("http://data.nba.com/data/1h/json/cms/noseason/scoreboard/{0}/games.json", gameDate);
            if (string.IsNullOrEmpty(data))
            {
                _logSvc.Info("UpdateGames did not receive data for {0}", dateTime.ToString());
                return;
            }
            var json = JObject.Parse(data);

            SqlParameter[] sqlParameters;
            const string sqlGames = @"
MERGE NbaGames s
USING (SELECT @GameId GameId) src
ON s.GameId = src.GameId
WHEN MATCHED THEN
UPDATE
SET SeasonId = @SeasonId, DateTime = @DateTime,
    PeriodValue = @PeriodValue, PeriodStatus = @PeriodStatus, PeriodName = @PeriodName, GameStatus = @GameStatus, GameClock = @GameClock,
    VisitorId = @VisitorId, VisitorScore = @VisitorScore, HomeId = @HomeId, HomeScore = @HomeScore,
    LastUpdated = CASE WHEN GameStatus = @GameStatus AND GameStatus = '3' THEN LastUpdated ELSE GETUTCDATE() END
WHEN NOT MATCHED THEN
INSERT (GameId, SeasonId, DateTime,
    PeriodValue, PeriodStatus, PeriodName, GameStatus, GameClock,
    VisitorId, VisitorScore, HomeId, HomeScore, LastUpdated)
VALUES (@GameId, @SeasonId, @DateTime,
    @PeriodValue, @PeriodStatus, @PeriodName, @GameStatus, @GameClock,
    @VisitorId, @VisitorScore, @HomeId, @HomeScore, GETUTCDATE());";

            DateTime minDateTime = dateTime.AddDays(-1),
                maxDateTime = dateTime.AddDays(1),
                maxLastUpdated = DateTime.UtcNow.AddHours(-1);
            var finalStatusGames = new HashSet<string>(
                _repo.GetList(game => game.DateTime >= minDateTime
                    && game.DateTime <= maxDateTime
                    && game.GameStatus.Equals("3")
                    && game.LastUpdated <= maxLastUpdated)
                .Select(game => game.GameId));

            foreach (var gameContent in json["sports_content"]["games"]["game"])
            {
                string gameId = GetJProperty<string>(gameContent, "id"), seasonId = GetJProperty<string>(gameContent, "season_id"), gameStatus = GetJProperty<string>(gameContent["period_time"], "game_status");

                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter { ParameterName = "@GameId", Value = gameId },
                    new SqlParameter { ParameterName = "@SeasonId", Value = seasonId },
                    new SqlParameter { ParameterName = "@DateTime", Value = DateTime.ParseExact(GetJProperty<string>(gameContent, "date") + GetJProperty<string>(gameContent, "time"), "yyyyMMddHHmm", null) },
                    new SqlParameter { ParameterName = "@PeriodValue", Value = GetJProperty<string>(gameContent["period_time"], "period_value") },
                    new SqlParameter { ParameterName = "@PeriodStatus", Value = GetJProperty<string>(gameContent["period_time"], "period_status") },
                    new SqlParameter { ParameterName = "@PeriodName", Value = GetJProperty<string>(gameContent["period_time"], "period_name") },
                    new SqlParameter { ParameterName = "@GameStatus", Value = gameStatus },
                    new SqlParameter { ParameterName = "@GameClock", Value = GetJProperty<string>(gameContent["period_time"], "game_clock") },
                    new SqlParameter { ParameterName = "@VisitorId", Value = GetJProperty<int>(gameContent["visitor"], "id") },
                    new SqlParameter { ParameterName = "@VisitorScore", Value = GetJProperty<string>(gameContent["visitor"], "score") },
                    new SqlParameter { ParameterName = "@HomeId", Value = GetJProperty<int>(gameContent["home"], "id") },
                    new SqlParameter { ParameterName = "@HomeScore", Value = GetJProperty<string>(gameContent["home"], "score") }
                };
                _sqlRepo.ExecuteSQL(sqlGames, sqlParameters);

                UpdateGameLinescore(gameContent, gameId, seasonId);

                foreach (var team in new string[] { "visitor", "home" })
                {
                    UpdateTeams(gameContent, team);
                }

                if (!gameStatus.Trim().Equals("1") && !finalStatusGames.Contains(gameId))
                {
                    var thread = new Thread(() => UpdateBoxscore(gameId, seasonId));
                    thread.Start();
                }
            }
        }

        private void UpdateGameLinescore(JToken gameContent, string gameId, string seasonId)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_GameLinescores")) return;

            const string sqlGameLinescores = @"
MERGE NbaGameLinescores s
USING (SELECT @GameId GameId, @TeamId TeamId, @PeriodValue PeriodValue) src
ON s.GameId = src.GameId AND s.TeamId = src.TeamId AND s.PeriodValue = src.PeriodValue
WHEN MATCHED THEN
UPDATE
SET PeriodName = @PeriodName, Score = @Score
WHEN NOT MATCHED THEN
INSERT (GameId, TeamId, PeriodValue, PeriodName, Score)
VALUES (@GameId, @TeamId, @PeriodValue, @PeriodName, @Score);";
            SqlParameter[] sqlParameters;
            foreach (var team in new string[] { "visitor", "home" })
            {
                if (gameContent[team]["linescores"] != null)
                {
                    if (gameContent[team]["linescores"]["period"] is JArray)
                    {
                        foreach (var lineScoreContent in gameContent[team]["linescores"]["period"])
                        {
                            sqlParameters = new SqlParameter[]
                            {
                                new SqlParameter { ParameterName = "@GameId", Value = gameId },
                                new SqlParameter { ParameterName = "@TeamId", Value = GetJProperty<int>(gameContent[team], "id") },
                                new SqlParameter { ParameterName = "@PeriodValue", Value = GetJProperty<string>(lineScoreContent, "period_value") },
                                new SqlParameter { ParameterName = "@PeriodName", Value = GetJProperty<string>(lineScoreContent, "period_name") },
                                new SqlParameter { ParameterName = "@Score", Value = GetJProperty<string>(lineScoreContent, "score") }
                            };
                            _sqlRepo.ExecuteSQL(sqlGameLinescores, sqlParameters);
                        }
                    }
                    else
                    {
                        // Only Q1 data in
                        sqlParameters = new SqlParameter[]
                        {
                            new SqlParameter { ParameterName = "@GameId", Value = gameId },
                            new SqlParameter { ParameterName = "@TeamId", Value = GetJProperty<int>(gameContent[team], "id") },
                            new SqlParameter { ParameterName = "@PeriodValue", Value = GetJProperty<string>(gameContent[team]["linescores"]["period"], "period_value") },
                            new SqlParameter { ParameterName = "@PeriodName", Value = GetJProperty<string>(gameContent[team]["linescores"]["period"], "period_name") },
                            new SqlParameter { ParameterName = "@Score", Value = GetJProperty<string>(gameContent[team]["linescores"]["period"], "score") }
                        };
                        _sqlRepo.ExecuteSQL(sqlGameLinescores, sqlParameters);
                    }
                }
            }
        }

        private void UpdateTeams(JToken gameContent, string team)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_Teams")) return;

            const string sqlTeams = @"
MERGE NbaTeams s
USING (SELECT @TeamId TeamId) src
ON s.TeamId = src.TeamId
WHEN MATCHED THEN
UPDATE
SET Abbreviation = @Abbreviation, City = @City, Nickname = @Nickname
WHEN NOT MATCHED THEN
INSERT (TeamId, Abbreviation, City, Nickname)
VALUES (@TeamId, @Abbreviation, @City, @Nickname);";
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@TeamId", Value = GetJProperty<int>(gameContent[team], "id") },
                new SqlParameter { ParameterName = "@Abbreviation", Value = GetJProperty<string>(gameContent[team], "abbreviation") },
                new SqlParameter { ParameterName = "@City", Value = GetJProperty<string>(gameContent[team], "city") },
                new SqlParameter { ParameterName = "@Nickname", Value = GetJProperty<string>(gameContent[team], "nickname") }
            };
            _sqlRepo.ExecuteSQL(sqlTeams, sqlParameters);
        }

        private void UpdateBoxscore(string gameId, string seasonId)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_Boxscores")) return;

            var data = _httpSvc.Get<string>("http://stats.nba.com/stats/boxscoretraditionalv2?EndPeriod=10&EndRange=28800&GameID={0}&RangeType=0&StartPeriod=1&StartRange=0", gameId);
            if (string.IsNullOrEmpty(data)) return;
            var json = JObject.Parse(data);

            var teamPlayers = new Dictionary<int, HashSet<int>>();
            var players = new Dictionary<int, string>();
            SqlParameter[] sqlParameters;
            const string sqlBoxscores = @"
MERGE NbaBoxscores s
USING (SELECT @GameId GameId, @TeamId TeamId, @PlayerId PlayerId) src
ON s.GameId = src.GameId AND s.TeamId = src.TeamId AND s.PlayerId = src.PlayerId
WHEN MATCHED THEN
UPDATE
SET StartPosition = @StartPosition, Min = @Min, FGM = @FGM, FGA = @FGA, FG3M = @FG3M, FG3A = @FG3A, FTM = @FTM, FTA = @FTA, OREB = @OREB, REB = @REB, AST = @AST, STL = @STL, BLK = @BLK, TOVR = @TOVR, PF = @PF, PTS = @PTS
WHEN NOT MATCHED THEN
INSERT (GameId, TeamId, PlayerId, StartPosition, Min, FGM, FGA, FG3M, FG3A, FTM, FTA, OREB, REB, AST, STL, BLK, TOVR, PF, PTS)
VALUES (@GameId, @TeamId, @PlayerId, @StartPosition, @Min, @FGM, @FGA, @FG3M, @FG3A, @FTM, @FTA, @OREB, @REB, @AST, @STL, @BLK, @TOVR, @PF, @PTS);";
            var mapCustomColumns = new Dictionary<string, string>
            {
                { "GAME_ID", "GameId" }, { "TEAM_ID", "TeamId" }, { "PLAYER_ID", "PlayerId" }, { "PLAYER_NAME", "PlayerName" }, { "START_POSITION", "StartPosition" }, { "TO", "TOVR" }
            };

            foreach (var resultSet in json["resultSets"])
            {
                if (!((string)resultSet["name"]).Equals("PlayerStats")) continue;
                var headers = resultSet["headers"].Select(header => mapCustomColumns.ContainsKey(((string)header).Trim()) ? mapCustomColumns[((string)header).Trim()] : ((string)header).Trim()).ToArray();
                sqlParameters = new SqlParameter[headers.Count()];
                foreach (var playerStats in resultSet["rowSet"])
                {
                    for (var i = 0; i < headers.Count(); i++)
                    {
                        var value = GetJProperty<string>(playerStats, i);
                        sqlParameters[i] = string.IsNullOrEmpty(value)
                            ? new SqlParameter { ParameterName = "@" + headers[i], Value = DBNull.Value }
                            : new SqlParameter { ParameterName = "@" + headers[i], Value = value };
                    }
                    _sqlRepo.ExecuteSQL(sqlBoxscores, sqlParameters);

                    var teamId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName.Equals("@TeamId")).Value);
                    var playerId = Convert.ToInt32(sqlParameters.First(p => p.ParameterName.Equals("@PlayerId")).Value);
                    if (!teamPlayers.ContainsKey(teamId))
                        teamPlayers[teamId] = new HashSet<int>();
                    teamPlayers[teamId].Add(playerId);
                    players[playerId] = sqlParameters.First(p => p.ParameterName.Equals("@PlayerName")).Value.ToString();
                }
            }
            UpdatePlayers(players);
            UpdateTeamPlayers(seasonId, teamPlayers);
        }

        private void UpdateTeamPlayers(string seasonId, Dictionary<int, HashSet<int>> teamPlayers)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_TeamPlayers")) return;

            const string sqlTeamPlayers = @"
MERGE NbaTeamPlayers s
USING (SELECT @SeasonId SeasonId, @TeamId TeamId, @PlayerId PlayerId) src
ON s.SeasonId = src.SeasonId AND s.TeamId = src.TeamId AND s.PlayerId = src.PlayerId
WHEN NOT MATCHED THEN
INSERT (SeasonId, TeamId, PlayerId)
VALUES (@SeasonId, @TeamId, @PlayerId);";
            foreach (KeyValuePair<int, HashSet<int>> team in teamPlayers)
            {
                foreach (var player in team.Value)
                {
                    var sqlParameters = new SqlParameter[3]
                    {
                        new SqlParameter { ParameterName = "@SeasonId", Value = seasonId },
                        new SqlParameter { ParameterName = "@TeamId", Value = team.Key },
                        new SqlParameter { ParameterName = "@PlayerId", Value = player }
                    };
                    _sqlRepo.ExecuteSQL(sqlTeamPlayers, sqlParameters);
                }
            }
        }

        private void UpdatePlayers(Dictionary<int, string> players)
        {
            if (!_cfgSvc.Get<bool>("Nba_Update_Players")) return;

            const string sqlPlayers = @"
MERGE NbaPlayers s
USING (SELECT @PlayerId PlayerId) src
ON s.PlayerId = src.PlayerId
WHEN MATCHED THEN
UPDATE
SET PlayerName = @PlayerName
WHEN NOT MATCHED THEN
INSERT (PlayerId, PlayerName)
VALUES (@PlayerId, @PlayerName);";
            foreach (KeyValuePair<int, string> player in players)
            {
                var sqlParameters = new SqlParameter[2]
                {
                    new SqlParameter { ParameterName = "@PlayerId", Value = player.Key },
                    new SqlParameter { ParameterName = "@PlayerName", Value = player.Value }
                };
                _sqlRepo.ExecuteSQL(sqlPlayers, sqlParameters);
            }
        }

        static T GetJProperty<T>(JToken token, int key)
        {
            try
            {
                object obj = token[key];
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(obj.ToString().Trim(), typeof(T));
                }
                return obj is T ? (T)obj : (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        static T GetJProperty<T>(JToken token, string key)
        {
            try
            {
                object obj = token[key];
                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(obj.ToString().Trim(), typeof(T));
                }
                return obj is T ? (T)obj : (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

    }
}
