﻿using System.Threading;
using System.Threading.Tasks;
using CodeManager.Data.Agent;
using CodeManager.Data.Events;
using MassTransit;

namespace CodeManagerAgent.Services
{
    public class AgentService : IAgentService
    {
        public string Token { get; set; }

        //Singleton
        public AgentState AgentState { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public Task SendAsync<T>(T message, ISendEndpoint sendEndpoint)
            where T : new()
        {
            if (message is ISecureMessage secureMessage) secureMessage.Token = Token;

            return sendEndpoint.Send(message);
        }
    }
}