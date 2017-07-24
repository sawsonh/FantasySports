using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FS.App.Mvc5.Admin.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Game Name must be between 5 and 20 characters")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Start Signup")]
        public DateTime RegistrationStartDate { get; set; }

        [Required]
        [DisplayName("End Signup")]
        public DateTime RegistrationEndDate { get; set; }

        [Required]
        [DisplayName("Start Game")]
        public DateTime PlayStartDate { get; set; }

        [Required]
        [DisplayName("End Game")]
        public DateTime PlayEndDate { get; set; }
    }

    public class GameLeagueViewModel
    {
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }

    public class GamePeriodViewModel
    {
        public int PeriodId { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Period Name must be between 4 and 15 characters")]
        public string Name { get; set; }

        public int? Value { get; set; }

        [Required]
        [DisplayName("Start Picks")]
        public DateTime PickStartDateTime { get; set; }

        [Required]
        [DisplayName("End Picks")]
        public DateTime PickEndDateTime { get; set; }

        [Required]
        [DisplayName("Start Reports")]
        public DateTime ReportStartDateTime { get; set; }

        [Required]
        [DisplayName("End Reports")]
        public DateTime ReportEndDateTime { get; set; }
    }
}