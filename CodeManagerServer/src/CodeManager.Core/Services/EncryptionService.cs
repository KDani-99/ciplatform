using System;
using System.Threading.Tasks;
using CodeManager.Data.Configuration;
using Microsoft.Extensions.Options;

namespace CodeManager.Data.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionConfiguration _encryptionConfiguration;
        
        public EncryptionService(IOptions<EncryptionConfiguration> encryptionConfiguration)
        {
            _encryptionConfiguration = encryptionConfiguration.Value ?? throw new ArgumentNullException(nameof(encryptionConfiguration));
        }
        
        public Task<string> EncryptAsync(string plain)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> DecryptAsync(string encrypted)
        {
            throw new System.NotImplementedException();
        }
    }
}