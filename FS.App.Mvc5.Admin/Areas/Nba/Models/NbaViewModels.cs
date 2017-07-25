using System;

namespace FS.App.Mvc5.Admin.Areas.Nba.Models
{
    public class NbaGameViewModel
    {
        public string GameId { get; set; }
        public DateTime DateTime { get; set; }
        public string Arena { get; set; }
        public string City { get; set; }
        public string Quarter { get; set; }
        public string Status { get; set; }
        public string GameClock { get; set; }
        public string Visitor { get; set; }
        public string VisitorScore { get; set; }
        public string Home { get; set; }
        public string HomeScore { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class NbaTeamViewModel
    {
        public int TeamId { get; set; }
        public string Abbreviation { get; set; }
        public string City { get; set; }
        public string Nickname { get; set; }
        public string Arena { get; set; }
    }

    public class NbaTeamStatViewModel
    {
        public string SeasonDesc { get; set; }
        public string Abbreviation { get; set; }
        public int? GP { get; set; }
        public int? W { get; set; }
        public int? L { get; set; }
        public double? FGPCT { get; set; }
        public double? FG3PCT { get; set; }
        public double? FTPCT { get; set; }
        public double? OREB { get; set; }
        public double? REB { get; set; }
        public double? AST { get; set; }
        public double? STL { get; set; }
        public double? BLK { get; set; }
        public double? TOVR { get; set; }
        public double? PTS { get; set; }
    }
}