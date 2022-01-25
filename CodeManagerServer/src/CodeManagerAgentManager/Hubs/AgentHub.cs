﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeManager.Core.Hubs.Clients;
using CodeManager.Core.Hubs.Messages;
using CodeManager.Data.Agent;
using CodeManager.Data.Entities.CI;
using CodeManager.Data.Events;
using CodeManager.Data.Repositories;
using CodeManagerAgentManager.Services;
using Microsoft.AspNetCore.SignalR;

namespace CodeManagerAgent.Hubs
{
    public class AgentHub : Hub<IAgentClient>
    {
        private readonly IRunRepository _runRepository;
        private readonly ILogStreamService _logStreamService;

        public AgentHub(IRunRepository runRepository, ILogStreamService logStreamService)
        {
            _runRepository = runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
        }

        [HubMethodName("SetAgentState")]
        public Task SetAgentStateAsync(AgentState agentState)
        {
            switch (agentState)
            {
                case AgentState.Available:
                    return Groups.AddToGroupAsync(Context.ConnectionId, nameof(AgentState.Available));
                case AgentState.Working:
                    return Groups.RemoveFromGroupAsync(Context.ConnectionId, nameof(AgentState.Available));
                default:
                    throw new ArgumentOutOfRangeException(nameof(agentState));
            }
        }
        
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Client connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("Client disconnected");
            return base.OnDisconnectedAsync(exception);
        }
        
        [HubMethodName("UploadLogStream")]
        public Task UploadLogStreamAsync(IAsyncEnumerable<string> stream, long runId, long jobId, int step)
        {
            return _logStreamService.WriteStreamAsync(stream, runId, jobId, step);
        }

    }
}