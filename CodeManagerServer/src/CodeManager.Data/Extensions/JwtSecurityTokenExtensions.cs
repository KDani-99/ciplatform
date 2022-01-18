using System.IdentityModel.Tokens.Jwt;

namespace CodeManager.Data.Extensions
{
    public static class JwtSecurityTokenExtensions
    {
        public static string ToBase64String(this JwtSecurityToken securityToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}