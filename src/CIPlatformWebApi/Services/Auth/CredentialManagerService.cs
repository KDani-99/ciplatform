using static BCrypt.Net.BCrypt;

namespace CIPlatformWebApi.Services.Auth
{
    public class CredentialManagerService : ICredentialManagerService
    {
        private const int WorkFactor = 12;

        public string CreateHashedPassword(string plain)
        {
            return HashPassword(plain, WorkFactor);
        }

        public bool VerifyPassword(string plain, string stored)
        {
            return Verify(plain, stored);
        }
    }
}