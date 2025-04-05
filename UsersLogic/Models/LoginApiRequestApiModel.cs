using System.ComponentModel.DataAnnotations;

namespace MySuperDuperPetProject.Models
{
    public class LoginApiRequestApiModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
