using System;
using CodeManager.Data.Configuration;
using CodeManagerAgent.Services;

namespace CodeManagerAgent.Factories
{
    public interface IJobHandlerServiceFactory
    {
        public IJobHandlerService Create(string token, JobConfiguration jobConfiguration, Uri responseAddress);
    }
}