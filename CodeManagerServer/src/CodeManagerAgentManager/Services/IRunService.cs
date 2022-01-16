using System;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;

namespace CodeManagerAgentManager.Services
{
    public interface IRunService
    {
        public Task StartAsync(StartRunCommand cmd);
    }
}