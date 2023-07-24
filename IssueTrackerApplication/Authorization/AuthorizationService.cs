using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IssueTracker.Abstractions.Definitions;

namespace IssueTracker.Application.Authorization
{
    public class AuthorizationService
    {
        private List<Claim> GenerateClaims(string role)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("scope", Scopes.IssuesRead),
                new Claim("scope", Scopes.IssuesWrite),
                new Claim("scope", Scopes.ProjectsRead)
            };

            if (role == "Admin")
            {
                claims.Add(new Claim("scope", Scopes.ProjectsWrite));
                claims.Add(new Claim("scope", Scopes.UsersRead));
                claims.Add(new Claim("scope", Scopes.UsersWrite));
            }

            return claims;
        }
        public string CreateToken(string role, string appSecret)
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
