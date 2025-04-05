namespace MySuperDuperPetProject.Models
{
    public class UserSessionApiResponseModel
    {
        public double JwtTimeToLive { get; set; }
        public double RefreshTimeToLive { get; set; }
        public string SessionId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Username { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
