using System.ComponentModel.DataAnnotations;

namespace UsersService.Models
{
    public class CreateRolesApiRequestModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public IEnumerable<string>? Roles { get; set; }
    }
}
