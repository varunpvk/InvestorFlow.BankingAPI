using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IF.Application.TokenService
{
    public class TokenCreator
    {
        private readonly IConfiguration _configuration;
        public TokenCreator(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = _configuration["Secrets:JwtKey"];

            var securekey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securekey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: "iss",
            audience: "aud",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
