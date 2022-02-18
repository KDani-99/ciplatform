namespace CIPlatformWebApi.Services.Auth
{
    public interface ICredentialManagerService
    {
        public string CreateHashedPassword(string plain);
        public bool VerifyPassword(string plain, string stored);
    }
}