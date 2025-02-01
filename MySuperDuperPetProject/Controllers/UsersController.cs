using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySuperDuperPetProject.Extensions;
using MySuperDuperPetProject.Middle;
using MySuperDuperPetProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


//05.01.2025

namespace MySuperDuperPetProject.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> logger, UsersLogic logic) : ControllerBase
    {
        

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([Required][FromBody] RegisterApiRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return BadRequest("Empty username");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Empty pas!");
            }
            return await logic.CreateUser(model.Username, PasswordHasher.HashPassword(model.Password), model.RoleId, HttpContext.RequestAborted) ? Ok() : BadRequest("Error on creating!");
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([Required][FromBody] LoginApiRequestApiModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return BadRequest("Empty username");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Empty pas!");
            }
            UserSessionApiResponseModel? token = await logic.LoginUser(model.Username, PasswordHasher.HashPassword(model.Password), HttpContext.RequestAborted);
            return token != null ? Ok(token) : NotFound();
        }
        [HttpPost("[action]/token")]
        [AllowAnonymous]
        public IActionResult Refresh([Required][FromQuery] string refreshToken, [Required][FromQuery] string sessionId)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("Empty token");
            }
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return BadRequest("Empty session id");
            }
            if (!User.GetUserId(out int userId))
            {
                return Unauthorized();
            }

            UserSessionApiResponseModel? response = logic.RefreshSession(refreshToken, sessionId, userId);
            return response != null ? Ok(response) : Unauthorized();
        }
       
        [HttpPut("change/password")]
        [Authorize(Roles = "Users.ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword([Required][FromBody] ChangePasswordApiRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                return BadRequest("Empty current pas");
            }
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest("Empty new pas");
            }

            string? username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; //берем username из JWT

            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized("There's no username in JWT!");
            }
            string? sessionId = User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return Unauthorized("There's no sessionId in JWT!");
            }
            

            return await logic.ChangeUserPassword(username, PasswordHasher.HashPassword(model.CurrentPassword), PasswordHasher.HashPassword(model.NewPassword), sessionId, HttpContext.RequestAborted) ? Ok() : NotFound();
        }
   
        [HttpGet("user/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Empty username");
            }

            UsersApiModel? user = await logic.GetUsernameFromDB(username, HttpContext.RequestAborted);
            return user != null ? Ok(user) : NotFound("User not found");
        }


        [HttpPost("role")]
        public async Task<IActionResult> CreateOrUpdateRole([Required][FromBody] CreateRolesApiRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Empty name");
            }
            if (!model.Roles?.Any() ?? true)
            {
                return BadRequest("Empty roles");
            }
            //TODO: Сделать проверку на соответствие присланных ролей системным
            return await logic.CreateOrUpdateRole(model.Name, model.Roles!) ? Ok() : StatusCode(500);
        }
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            IReadOnlyList<RoleApiModel>? roles = await logic.GetRoles(HttpContext.RequestAborted);
            return roles != null && roles.Count > 0 ? Ok(roles) : StatusCode(500);
        }
        [HttpGet("roles/system")]
        public IActionResult GetSystemRoles()
        {
            return Ok(RolesCollectionStorage.Roles);
        }
    }
  




        }
