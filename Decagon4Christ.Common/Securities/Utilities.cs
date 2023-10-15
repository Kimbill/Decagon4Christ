using Decagon4Christ.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Decagon4Christ.Common.Securities
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;
        public Utilities(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwt(User user)
        {
            var listOfClaims = new List<Claim>();

            listOfClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            listOfClaims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));

            var key = Generate256BitKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(listOfClaims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(createdToken);

            return token;
        }

        private byte[] Generate256BitKey()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[32]; // 32 bytes for 256 bits
                randomNumberGenerator.GetBytes(randomNumber);
                return randomNumber;
            }
        }
    }
}
