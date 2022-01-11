namespace CodeManagerWebApi.DataTransfer
{
    public record AuthTokenDto
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}