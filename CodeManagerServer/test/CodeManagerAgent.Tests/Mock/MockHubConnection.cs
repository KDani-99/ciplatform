using System;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeManagerAgent.Tests.Mocks
{
    public class MockHubConnection : HubConnection
    {
        public MockHubConnection() : base(new Mock<IConnectionFactory>().Object, new Mock<IHubProtocol>().Object,
                                          new Mock<EndPoint>().Object, new Mock<IServiceProvider>().Object,
                                          new Mock<ILoggerFactory>().Object)
        {
        }

        private MockHubConnection(IConnectionFactory connectionFactory,
                                  IHubProtocol hubProtocol,
                                  EndPoint endpoint,
                                  IServiceProvider serviceProvider,
                                  ILoggerFactory loggerFactory) : base(connectionFactory, hubProtocol, endpoint,
                                                                       serviceProvider, loggerFactory)
        {
        }
    }
}