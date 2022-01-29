using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManager.Data.Extensions;
using CodeManager.Data.JsonWebTokens;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Exceptions;

namespace CodeManagerAgentManager.Services
{
    public class JobService : IJobService<AcceptedRequestJobCommandResponse>
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IRunRepository _runRepository;
        private readonly ITokenService<JwtSecurityToken> _tokenService;
        private readonly IVariableService _variableService;

        public JobService(JsonSerializerOptions jsonSerializerOptions, IRunRepository runRepository,
            IVariableService variableService,
            ITokenService<JwtSecurityToken> tokenService)
        {
            _jsonSerializerOptions =
                jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _variableService = variableService ?? throw new ArgumentNullException(nameof(variableService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<AcceptedRequestJobCommandResponse> ProcessJobRequestTokenAsync(string requestToken)
        {
            var token = await _tokenService.VerifyJobRequestTokenAsync(requestToken);
            var runId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.RunId)
                .Value);
            var jobId = long.Parse(token.Claims.First(claim => claim.Type == CustomJwtRegisteredClaimNames.JobId)
                .Value);

            var run = await _runRepository.GetAsync(runId) ?? throw new RunDoesNotExistException();

            if (run.State == States.Failed)
                // TODO: run requires different states, it cant be "Skipped"
                throw new RunFailedException();

            var job = run.Jobs.First(item => item.Id == jobId);

            if (job.State != States.Queued)
                throw new RunAlreadyStartedException(); // job already started by a different consumer

            run.State = States.Running;

            job.State = States.Running;
            job.StartDateTime = DateTime.Now;

            await _runRepository.UpdateAsync(run);

            // TODO: add variable service that returns the decrypted variables
            var variables = await _variableService.GetVariablesForProject(run.Project.Id);

            var jobToken = await _tokenService.CreateJobTokenAsync(runId, jobId);
            var jobConfiguration =
                ParseSecretsAsync(JsonSerializer.Deserialize<JobConfiguration>(job.JsonContext, _jsonSerializerOptions),
                    variables.ToList());

            return new AcceptedRequestJobCommandResponse
            {
                Token = jobToken.ToBase64String(),
                JobConfiguration = jobConfiguration,
                Repository = run.Repository
            };
        }

        private static JobConfiguration ParseSecretsAsync(JobConfiguration jobConfiguration, IList<Variable> variables)
        {
            // modifies the state and returns the reference to the given object
            foreach (var step in jobConfiguration.Steps)
            {
                var sb = new StringBuilder(step.Cmd); // StringBuilder is better for repeated replace calls
                foreach (var variable in variables) sb.Replace($"$({variable.Name})", variable.Value);
            }

            return jobConfiguration;
        }
    }
}