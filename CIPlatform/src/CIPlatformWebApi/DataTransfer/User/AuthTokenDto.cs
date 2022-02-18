namespace CIPlatformWebApi.DataTransfer.User
{
    public record AuthTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}