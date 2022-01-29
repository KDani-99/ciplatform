using System.Threading.Tasks;

namespace CodeManager.Core.Services
{
    public interface IEncryptionService
    {
        public Task<string> EncryptAsync(string plain);
        public Task<string> DecryptAsync(string encrypted);
    }
}