using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySuperDuperPetProject.Extensions;
using MySuperDuperPetProject.Middle;
using MySuperDuperPetProject.Models;

namespace MySuperDuperPetProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class WebTransferController(ILogger<WebTransferController> logger, ITransferLogic logic) : ControllerBase
    {
        [HttpPost("transfer/{from}/{to}/")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostTransfer([Required] [FromRoute] string from,
            [Required] [FromRoute] string to)
        {
            string? username = User.GetUsername();
            if (string.IsNullOrWhiteSpace(username) || !User.GetUserId(out int userId))
            {
                return Unauthorized();
            }

            logger.LogInformation("Get user request. Transfer from {from} to {to} by user {user}", from, to, username);
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("����������� ������ ��������!");
            }

            if (await logic.PostTransfer(userId, username, from, to, HttpContext.RequestAborted))
            {
                return Ok();
            }

            return StatusCode(500, "Internal database error!");
        }

        [HttpGet("user/transfers")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserTransfersByPeriod([Required] [FromQuery] DateTimeOffset from,
            [Required] [FromQuery] DateTimeOffset to)
        {
            string? username = User.GetUsername();
            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized();
            }

            logger.LogInformation("Get user request. Get user transfers by user {user} and period from {from} to {to}",
                username, from, to);
            if (from > to)
            {
                return BadRequest("����������� ����� ��������� ������!");
            }

            IEnumerable<TransferResponseModel>? trans = await logic.GetTransfers(from, to, HttpContext.RequestAborted);
            if (trans == null)
            {
                return StatusCode(500, "Internal database error!");
            }

            return Ok(trans);
        }

        [HttpGet("transfers/{count}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostPopularTransfers([Required] [FromRoute] int count)
        {
            if (count <= 0)
            {
                return BadRequest("���-�� ��������� �� ������ ���� ������ ��� ����� ����!");
            }

            IEnumerable<TransferStatisticResponseModel>? res =
                await logic.GetMostPopularTransfer(count, HttpContext.RequestAborted);
            if (res == null)
            {
                return StatusCode(500, "Internal database error!");
            }

            return Ok(res);
        }
    }
}