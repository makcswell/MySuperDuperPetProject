namespace MySuperDuperPetProject.Models
{
    public class RoleApiModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
}
