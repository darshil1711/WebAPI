using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Implementation
{
    public class TokenService : ITokenServices
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatejWTToken(ClaimsIdentity claimsIdentity)
        {
            try
            {
                // Create claims
                var Authclaim = new List<Claim>(claimsIdentity.Claims);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims: Authclaim,
                    expires: DateTime.Now.AddHours(15),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Something Went Wrong" + ex.Message);
            }
        }
    }
}
