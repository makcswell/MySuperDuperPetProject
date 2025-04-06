using System.ComponentModel.DataAnnotations;
using CustomExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaticStorages;
using UsersService.Middle;
using UsersService.Models;

namespace UsersService.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController(ILogger<UsersController> logger, UsersLogic logic) : ControllerBase
{
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([Required] [FromBody] RegisterApiRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            return BadRequest("Empty username");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest("Empty pas!");
        }

        return await logic.CreateUser(model.Username, PasswordHasher.HashPassword(model.Password), model.RoleId,
            HttpContext.RequestAborted)
            ? Ok()
            : BadRequest("Error on creating!");
    }

    [HttpPost("[action]")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([Required] [FromBody] LoginApiRequestApiModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            return BadRequest("Empty username");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            return BadRequest("Empty pas!");
        }

        UserSessionApiResponseModel? token = await logic.LoginUser(model.Username,
            PasswordHasher.HashPassword(model.Password), HttpContext.RequestAborted);
        return token != null ? Ok(token) : NotFound();
    }

    [HttpPost("[action]/token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Refresh([Required] [FromQuery] string refreshToken,
        [Required] [FromQuery] string sessionId)
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
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeUserPassword([Required] [FromBody] ChangePasswordApiRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.CurrentPassword))
        {
            return BadRequest("Empty current pas");
        }

        if (string.IsNullOrWhiteSpace(model.NewPassword))
        {
            return BadRequest("Empty new pas");
        }

        string? username = User.GetUsername();

        if (string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized("Please regenerate token");
        }

        string? sessionId = User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized("There's no sessionId in JWT!");
        }

        return await logic.ChangeUserPassword(username, PasswordHasher.HashPassword(model.CurrentPassword),
            PasswordHasher.HashPassword(model.NewPassword), sessionId, HttpContext.RequestAborted)
            ? Ok()
            : NotFound();
    }

    [HttpGet("user/{username}")]
    [Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdateRole([Required] [FromBody] CreateRolesApiRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return BadRequest("Empty name");
        }

        if (!model.Roles?.Any() ?? true)
        {
            return BadRequest("Empty roles");
        }

        List<string> invalidRoles = (model.Roles ?? []).Except(RolesCollectionStorage.Roles)
            .ToList();
        if (invalidRoles.Count > 0)
        {
            return BadRequest("Roles not equals to system roles");
        }

        return await logic.CreateOrUpdateRole(model.Name, model.Roles!) ? Ok() : StatusCode(500);
    }

    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRoles()
    {
        IReadOnlyList<RoleApiModel>? roles = await logic.GetRoles(HttpContext.RequestAborted);
        return roles != null && roles.Count > 0 ? Ok(roles) : StatusCode(500);
    }

    [HttpGet("roles/system")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSystemRoles()
    {
        return Ok(RolesCollectionStorage.Roles);
    }
}