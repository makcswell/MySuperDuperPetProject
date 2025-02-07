using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySuperDuperPetProject.Extensions;
using MySuperDuperPetProject.Middle;
using MySuperDuperPetProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MySuperDuperPetProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WebTransferController(ILogger<WebTransferController> logger, ITransferLogic logic) : ControllerBase
    {

        private string? GetUsernameFromToken()
        {
            string? username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return username;// Извлекаем username из токена
        }

        [HttpPost("transfer/{from}/{to}/")]
        [ProducesResponseType(typeof(string), 502)]
        public async Task<IActionResult> PostTransfer([Required][FromRoute] string from, [Required][FromRoute] string to)// добавил username
        {
            string? username = GetUsernameFromToken();
            if (string.IsNullOrWhiteSpace(username) || !User.GetUserId(out int userId))
            {
                return Unauthorized();
            }
            logger.LogInformation("Get user request. Transfer from {from} to {to} by user {user}", from, to, username);
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("Некорректно заданы переходы!");
            }
            if (await logic.PostTransfer(userId, username, from, to, HttpContext.RequestAborted))
            {
                return Ok();
            }
            return StatusCode(502, "Internal database error!");
        }
        [HttpGet("user/transfers")]
        [ProducesResponseType(typeof(IEnumerable<>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 502)]
        public async Task<IActionResult> GetUserTransfersByPeriod([Required][FromQuery] DateTimeOffset from, [Required][FromQuery] DateTimeOffset to)//добавил username
        {
            string? username = GetUsernameFromToken();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized();
            }
            logger.LogInformation("Get user request. Get user transfers by user {user} and period from {from} to {to}", username, from, to);
            if (from > to)
            {
                return BadRequest("Некорреткно задан временной период!");
            }
            IEnumerable<TransferResponseModel>? trans = await logic.GetTransfers(from, to, HttpContext.RequestAborted);
            if (trans == null)
            {
                return StatusCode(502, "Internal database error!");
            }
            return Ok(trans);
        }
        [HttpGet("transfers/{count}")]
        [ProducesResponseType(typeof(IEnumerable<TransferStatisticResponseModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 502)]
        public async Task<IActionResult> GetMostPopularTransfers([Required][FromRoute] int count)
        {
            if (count <= 0)
            {
                return BadRequest("Кол-во переходов не должно быть меньше или равно нулю!");
            }
            IEnumerable<TransferStatisticResponseModel>? res = await logic.GetMostPopularTransfer(count, HttpContext.RequestAborted);
            if (res == null)
            {
                return StatusCode(502, "Internal database error!");
            }
            return Ok(res);
        }
    }
}
