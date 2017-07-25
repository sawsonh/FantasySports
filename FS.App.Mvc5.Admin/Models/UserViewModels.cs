using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FS.App.Mvc5.Admin.Models
{
    public class UserViewModel
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        public int Id { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Signup Date")]
        public DateTime CreateDateTime { get; set; }

        [Required]
        [DisplayName("Id Provider")]
        public string IdentityProviderName { get; set; }
    }
}