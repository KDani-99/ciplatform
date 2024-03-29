﻿using System;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Services;
using CIPlatformWebApi.Services.Run;
using MassTransit.JobService.Components;

namespace CIPlatformWebApi.Strategies
{
    public class JobResultChannelConnectionHandler : IResultChannelConnectionHandler
    {
        private readonly IRunService _runService;
        
        public JobResultChannelConnectionHandler(IRunService runService)
        {
            _runService = runService ?? throw new ArgumentNullException(nameof(runService));
        }
        
        public Task<bool> VerifyAsync(long entityId, UserEntity user)
        {
            return _runService.IsAllowedJob(entityId, user);
        }
    }
}