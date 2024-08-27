using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TrainerApi.Services
{
    public class JwtService
    {
        private readonly SymmetricSecurityKey _signingKey;
        private readonly SigningCredentials _signingCredentials;

        public JwtService(IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Secret"];
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            _signingKey = new SymmetricSecurityKey(keyBytes);
            _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(string username, bool is2FAEnabled)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("Is2FAEnabled", is2FAEnabled.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = _signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = _signingKey.Key; // Use the key bytes from the SymmetricSecurityKey

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var securityToken);

                return principal;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return null;
            }
        }
    }
}
