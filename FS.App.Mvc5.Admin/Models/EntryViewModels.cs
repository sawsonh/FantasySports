using System;

namespace FS.App.Mvc5.Admin.Models
{
    public class EntryViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int GameId { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}