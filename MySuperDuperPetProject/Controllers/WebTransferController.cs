using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private string GetUsernameFromToken()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return username;// Извлекаем username из токена
        }

        [HttpPost("transfer/{from}/{to}/")]
        public async Task<IActionResult> PostTransfer([Required][FromRoute] string from, [Required][FromRoute] string to)// добавил username
        {
            var username = GetUsernameFromToken();
            logger.LogInformation("Get user request. Transfer from {from} to {to} by user {user}", from, to, username);
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("Некорректно заданы переходы!");
            }
            /*   if (username != )
               {
                   return BadRequest("Пользователь не авторизован!");
               }*/
            if (await logic.PostTransfer(username, from, to, HttpContext.RequestAborted))
            {
                return Ok();
            }
            return StatusCode(502, "Internal database error!");
        }
        [HttpGet("user/transfers")]
        public async Task<IActionResult> GetUserTransfersByPeriod([Required][FromQuery] DateTimeOffset from, [Required][FromQuery] DateTimeOffset to)//добавил username
        {
            var username = GetUsernameFromToken();
            logger.LogInformation("Get user request. Get user transfers by user {user} and period from {from} to {to}", username, from, to);
            /*  if (username != model.Token)
              {
                  return BadRequest("Идентификатор пользователя не может быть меньше нуля!");
              }*/
            if (from > to)
            {
                return BadRequest("Некорреткно задан временной период!");
            }
            IEnumerable<TransferResponseModel>? trans = await logic.GetTransfers(username, from, to, HttpContext.RequestAborted);
            if (trans == null)
            {
                return StatusCode(502, "Internal database error!");
            }
            return Ok(trans);
        }
        [HttpGet("transfers/{count}")]
        public async Task<IActionResult> GetMostPopularTransfers([Required][FromRoute] int count)
        {
            var username = GetUsernameFromToken();
            if (count <= 0)
            {
                return BadRequest("Кол-во переходов не должно быть меньше или равно нулю!");
            }
            IEnumerable<TransferStatisticResponseModel>? res = await logic.GetMostPopularTransfer(username, count, HttpContext.RequestAborted);
            if (res == null)
            {
                return StatusCode(502, "Internal database error!");
            }
            return Ok(res);
        }
    }
}
