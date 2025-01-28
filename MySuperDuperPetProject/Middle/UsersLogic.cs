using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MySuperDuperPetProject.Models;
using MySuperDuperPetProject.TransferDatabaseContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MySuperDuperPetProject.Middle
{
    public class UsersLogic(ILogger<UsersLogic> logger, TransferDbContext db, IConfiguration config, IMemoryCache cache)
    {
        private readonly TimeSpan tokenLifeTime = TimeSpan.FromMinutes(int.Parse(config["TokenLifeTime"] ?? "15"));
        private readonly TimeSpan refreshLifeTime = TimeSpan.FromMinutes(int.Parse(config["RefreshTokenLifeTime"] ?? "30"));
        private readonly string jwtIssuer = config["JwtIssuer"]!;
        public async Task<bool> CreateUser(string username, string password, int roleId, CancellationToken token = default)
        {
            try
            {
                User? user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == username, token);
                if (user != null)
                {
                    logger.LogWarning("User {username} is already exists!", username);
                    return false;
                }
                Role? role = await db.Roles.FirstOrDefaultAsync(r => r.Id == roleId, token);
                if (role == null)
                {
                    logger.LogWarning("Role with id {id} does not exists!", roleId);
                    return false;
                }
                User newUser = new()
                {
                    Name = username,
                    Password = password,
                    Role = role,
                };
                await db.Users.AddAsync(newUser, token);
                await db.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on creating new user!");
                return false;
            }
        }

        public async Task<UserSessionApiResponseModel?> LoginUser(string username, string password, CancellationToken token = default)
        {
            try
            {
                User? user = await db.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.Name == username && u.Password == password, token);
                if (username == null)
                {
                    return null;
                }
                string sessionId = Guid.NewGuid().ToString("N");
                return GenerateToken(user.Name, user.Role.Roles, sessionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on logining user!");
                return null;
            }
        }

        public UserSessionApiResponseModel? RefreshSession(string refreshToken, string oldSessiondId)
        {
            if (!cache.TryGetValue(oldSessiondId, out UserSessionApiResponseModel? value) || value == null || refreshToken != value.RefreshToken)
            {
                return null;
            }
            string newSessionId = Guid.NewGuid().ToString("N");
            UserSessionApiResponseModel result = GenerateToken(value.Username, value.Roles, newSessionId);
            cache.Remove(oldSessiondId);
            return result;
        }

        private UserSessionApiResponseModel GenerateToken(string username, IEnumerable<string> roles, string sessionId)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, username),
                new("SessionId", sessionId),
                new Claim("Constant", "sas")
            ];
            roles.ToList().ForEach(r =>
            {
                claims.Add(new(ClaimTypes.Role, r));
            });

            SymmetricSecurityKey key = new(Convert.FromBase64String(SecretKeyStorage.SecretKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            DateTime expires = DateTime.UtcNow.Add(tokenLifeTime);

            JwtSecurityToken token = new(jwtIssuer, null, claims, null, expires, creds);

            string refreshToken = Guid.NewGuid().ToString("N");

            UserSessionApiResponseModel result = new()
            {
                JwtTimeToLive = tokenLifeTime.TotalSeconds,
                RefreshTimeToLive = refreshLifeTime.TotalSeconds,
                SessionId = sessionId,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Roles = roles,
                Username = username,
            };

            cache.Set(sessionId, result, DateTimeOffset.UtcNow.Add(refreshLifeTime));

            return result;
        }

        public async Task<bool> ChangeUserPassword(string username, string oldpass, string newpass, string sessionId, CancellationToken token = default)//добавил username для взятия из таблицы
        {
            try
            {
                User? user = await db.Users.FirstOrDefaultAsync(u => u.Name == username && u.Password == oldpass, token);

                if (user == null)
                {
                    return false;
                }
                user.Password = newpass;
                await db.SaveChangesAsync(token);

                cache.Remove(sessionId);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on logining user!");
                return false;
            }
        }

        public async Task<bool> CreateOrUpdateRole(string name, IEnumerable<string> roles, CancellationToken token = default)
        {
            try
            {
                Role? role = await db.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken: token);
                if (role == null)
                {
                    Role newRole = new()
                    {
                        Name = name,
                        Roles = roles
                    };
                    await db.Roles.AddAsync(newRole, token);
                    await db.SaveChangesAsync(token);
                    return true;
                }
                role.Roles = roles;
                await db.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on creating or updating role!");
                return false;
            }
        }


        public async Task<UsersApiModel?> GetUsernameFromDB(string username, CancellationToken token = default)
        {

            try
            {
                User? user = await db.Users.FirstOrDefaultAsync(u => u.Name == username, token);
                if (user == null)
                {
                    return null;
                }
                return new UsersApiModel
                {
                    Id = user.Id,
                    username = user.Name,

                };

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Ошибка в импорте username");
                return null;
            }
        }


        public async Task<IReadOnlyList<RoleApiModel>?> GetRoles(CancellationToken token = default)
        {
            try
            {
                return (await db.Roles.AsNoTracking().ToListAsync(cancellationToken: token)).Select(r => new RoleApiModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Roles = r.Roles
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on getting roles list!");
                return null;
            }
        }
    }
}
