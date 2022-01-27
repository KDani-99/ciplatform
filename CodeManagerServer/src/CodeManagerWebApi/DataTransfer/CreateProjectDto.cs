using System.Collections.Generic;
using CodeManager.Data.Entities;
using CodeManager.Data.Entities.CI;

namespace CodeManagerWebApi.DataTransfer
{
    public class CreateProjectDto
    {
        public string RepositoryUrl { get; set; }
        public bool IsPrivateRepository { get; set; }
        public bool IsPrivateProject { get; set; }
        public bool IsSSH { get; set; }
        public string SecretToken { get; set; } // Either SSH or access token
        public long TeamId { get; set; } // owner team id
    }
}