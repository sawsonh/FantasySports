using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FS.App.Mvc5.Admin.Models
{
    public class IdentityProviderViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("App Id")]
        public string AppId { get; set; }
    }
}