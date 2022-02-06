namespace CodeManagerWebApi.Configuration
{
    public class JwtConfiguration
    {
        public int AccessTokenLifeTime { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string Secret { get; set; }
    }
}