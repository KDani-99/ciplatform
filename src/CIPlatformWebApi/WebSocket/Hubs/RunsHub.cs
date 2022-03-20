using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CIPlatform.Data.Entities;
using CIPlatformWebApi.Exceptions;
using CIPlatformWebApi.Strategies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CIPlatformWebApi.WebSocket.Hubs
{
    [Authorize(Roles = "User,Admin")]
    public sealed class RunsHub : Hub
    {
        public static readonly IReadOnlyDictionary<string, Func<long, string>> AvailableResultsChannels = new Dictionary<string, Func<long, string>>
        {
            { "project", entityId => $"Project:{entityId}" },
            { "run", entityId => $"Run:{entityId}" },
            { "job", entityId => $"Job:{entityId}" },
            { "step", entityId => $"Step:{entityId}" }
        };
        private readonly IResultChannelConnectionHandlerFactory _resultChannelConnectionHandlerFactory;

        public RunsHub(IResultChannelConnectionHandlerFactory resultChannelConnectionHandlerFactory)
        {
            _resultChannelConnectionHandlerFactory = resultChannelConnectionHandlerFactory ??
                throw new ArgumentNullException(nameof(resultChannelConnectionHandlerFactory));
        }
        
        [HubMethodName("SubscribeToResultsChannel")]
        public async Task SubscribeToResultsChannelAsync(string resultsChannel, long entityId)
        {
            var user = Context.GetHttpContext().Items["user"] as UserEntity;

            var handler = _resultChannelConnectionHandlerFactory.Create(resultsChannel);

            if (!await handler.VerifyAsync(entityId, user))
            {
                throw new UnauthorizedAccessWebException("You are not allowed to enter to this channel.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, AvailableResultsChannels[resultsChannel](entityId));
        }
        
        [HubMethodName("UnSubscribeFromResultsChannel")]
        public Task UnSubscribeFromResultsChannelAsync(string resultsChannel, long entityId)
        {
            if (!AvailableResultsChannels.ContainsKey(resultsChannel))
            {
                throw new ChannelDoesNotExistException(resultsChannel);
            }
            
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, AvailableResultsChannels[resultsChannel](entityId));
        }
    }
}