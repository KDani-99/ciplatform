using System;
using System.Threading.Tasks;
using CodeManagerWebApi.Entities;
using CodeManagerWebApi.Entities.Configuration;
using CodeManagerWebApi.Exceptions;
using CodeManagerWebApi.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeManagerWebApi.Services
{
    public class BootstrapService
    {
        private readonly ICredentialManagerService _credentialManagerService;
        private readonly ILogger<BootstrapService> _logger;
        private readonly IUserRepository _userRepository;

        public BootstrapService(
            IUserRepository userRepository,
            ICredentialManagerService credentialManagerService,
            ILogger<BootstrapService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _credentialManagerService = credentialManagerService ??
                                        throw new ArgumentNullException(nameof(credentialManagerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(ServiceAccountConfiguration serviceAccountConfiguration)
        {
            if (!serviceAccountConfiguration.IsEnabled) return;

            if (await _userRepository.GetByUsernameAsync(serviceAccountConfiguration
                .Username) is not null) // TODO: replace with ExistsAsync (helper method)
            {
                _logger.LogWarning(
                    $"Service account with name `{serviceAccountConfiguration.Username}` could not be created: {new UsernameTakenException().Message}");
                return;
            }

            var serviceUser = new User
            {
                Username = serviceAccountConfiguration.Username,
                Password = _credentialManagerService.CreateHashedPassword(serviceAccountConfiguration.Password),
                Roles = new[] {Roles.User, Roles.Admin},
                IsActive = true,
                Email = "site.admin@noreply",
                RegistrationTimestamp = DateTime.Now
            };

            await _userRepository.CreateAsync(serviceUser);
            _logger.LogInformation($"Service account `{serviceUser.Username}` created");
        }
    }
}