namespace CodeManagerWebApi.Entities
{
    public class AuthResponse
    {
        public JwtAuthToken AccessToken { get; set; }
        public JwtAuthToken RefreshToken { get; set; }
    }
}