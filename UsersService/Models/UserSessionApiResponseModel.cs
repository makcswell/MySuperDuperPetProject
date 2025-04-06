namespace UsersService.Models
{
    public class UserSessionApiResponseModel
    {
        public double JwtTimeToLive { get; set; }
        public double RefreshTimeToLive { get; set; }
        public required string SessionId { get; set; }
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Username { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
