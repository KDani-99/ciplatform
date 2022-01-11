using System.IdentityModel.Tokens.Jwt;

namespace CodeManagerWebApi.Utils.Extensions
{
    public static class JwtTokenExtensions
    {
        public static string ToBase64String(this JwtSecurityToken securityToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}