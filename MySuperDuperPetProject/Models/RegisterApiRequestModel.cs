using System.ComponentModel.DataAnnotations;

namespace MySuperDuperPetProject.Models
{
    public class RegisterApiRequestModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
