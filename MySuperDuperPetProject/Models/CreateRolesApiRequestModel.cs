using System.ComponentModel.DataAnnotations;

namespace MySuperDuperPetProject.Models
{
    public class CreateRolesApiRequestModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public IEnumerable<string>? Roles { get; set; }
    }
}
