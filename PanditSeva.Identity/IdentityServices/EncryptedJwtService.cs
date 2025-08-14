using Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace PanditSeva.Identity.IdentityServices
{
    public class EncryptedJwtService
    {
        private readonly IConfiguration _config;

        public EncryptedJwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateEncryptedToken(ApplicationUser user)
        {
            var handler = new JsonWebTokenHandler();

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var encryptKey = new SymmetricSecurityKey(key);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Claims = claims.ToDictionary(c => c.Type, c => (object)c.Value),
                Expires = DateTime.UtcNow.AddHours(2),
                EncryptingCredentials = new EncryptingCredentials(
                    encryptKey,
                    SecurityAlgorithms.Aes256KW,
                    SecurityAlgorithms.Aes256CbcHmacSha512),
                SigningCredentials = new SigningCredentials(encryptKey, SecurityAlgorithms.HmacSha256)
            };

            return handler.CreateToken(descriptor);
        }

        public ClaimsPrincipal? DecryptToken(string token)
        {
            var handler = new JsonWebTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                TokenDecryptionKey = new SymmetricSecurityKey(key),
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true
            };

            var result = handler.ValidateToken(token, validationParams);
            return result.IsValid ? result.ClaimsIdentity != null ? new ClaimsPrincipal(result.ClaimsIdentity) : null : null;
        }
    }

}
