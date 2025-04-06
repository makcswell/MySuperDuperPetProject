using System.ComponentModel.DataAnnotations;

namespace UsersService.Models
{
    public class RegisterApiRequestModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
