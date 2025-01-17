using System.ComponentModel.DataAnnotations;

namespace MySuperDuperPetProject.Models
{
    public class ChangePasswordApiRequestModel
    {
        [Required]
        public required string CurrentPassword { get; set; }
        [Required]
        public required string NewPassword { get; set; }
    }
}
