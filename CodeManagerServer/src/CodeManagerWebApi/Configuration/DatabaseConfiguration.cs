using System.Collections;
using System.Collections.Generic;

namespace CodeManagerWebApi.Entities.Configuration
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public IDictionary<string, string> Collections { get; set; }
    }
}