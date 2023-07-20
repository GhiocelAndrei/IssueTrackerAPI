using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IssueTracker.Application.Services
{
    public class AuthorizationService
    {
        private List<Claim> GenerateClaims(string role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("access_issues", "enabled"),
                new Claim("read_projects", "enabled")
            };

            if (role == "Admin")
            {
                claims.Add(new Claim("write_projects", "enabled"));
                claims.Add(new Claim("access_users", "enabled"));
            }

            return claims;
        }
        public string CreateToken(String role, string appSecret)
        {
            List<Claim> claims = GenerateClaims(role);

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                appSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
