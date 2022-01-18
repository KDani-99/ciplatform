using System.IdentityModel.Tokens.Jwt;

namespace CodeManager.Data.Extensions
{
    public static class StringExtensions
    {
        public static JwtSecurityToken DecodeJwtToken(this string token)
        {
            var handler = new JwtSecurityTokenHandler {MapInboundClaims = false};
            return handler.ReadJwtToken(token);
        }
    }
}