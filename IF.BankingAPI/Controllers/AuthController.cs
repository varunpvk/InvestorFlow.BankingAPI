using IF.Application.TokenService;
using IF.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IF.BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Banking API")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenCreator _tokenCreator;

        public AuthController(TokenCreator tokenCreator)
        {
            _tokenCreator = tokenCreator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if(model.Username == "admin" && model.Password == "admin")
            {
                var token = _tokenCreator.GenerateToken(model.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }
}
